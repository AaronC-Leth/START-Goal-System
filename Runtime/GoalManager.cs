using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using START.scripts.GoalSystem.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace START.Scripts.GoalSystem
{
    [ExecuteAlways]
    public class GoalManager : SerializedMonoBehaviour
    {
        private static GoalManager _instance;
        private static bool _isQuitting = false;

        
        #region START_Debug

        [FoldoutGroup("Debug")] [SerializeField]
        private bool debugActive;

        [FoldoutGroup("Debug")] [SerializeField]
        private Color debugColor = Color.cyan;
        
        [FoldoutGroup("Debug")] [SerializeField]
        private Color warnColor = Color.yellow;

        private void Log(string title, string body = "", bool isWarning = false)
        {
            if(!debugActive) return;
            string color = isWarning ? warnColor.ToHexString() : debugColor.ToHexString();
            string titleMsg = title.ToUpper();
            string message = $"<color='#{color}'>{titleMsg}</color> {body}";
            Debug.Log(message);
        }

        #endregion

        
        public static GoalManager Instance
        {
            get
            {
                if (_isQuitting)
                {
                    return null;
                }

                if (_instance == null)
                {
                    if (!Application.isPlaying)
                    {
                        Debug.LogWarning("GoalManager Instance requested outside of play mode. Returning null.");
                        return null;
                    }

                    _instance = FindObjectOfType<GoalManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GoalManager");
                        _instance = go.AddComponent<GoalManager>();
                    }
                }
                return _instance;
            }
        }
        
        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }


        [SerializeField, ReadOnly]
        private Dictionary<GoalSO, GoalData> goalDataMap = new Dictionary<GoalSO, GoalData>();

        [SerializeField] // Make these serializable so they appear in the inspector
        public UnityEvent<GoalSO> OnGoalActivated;
        [SerializeField]
        public UnityEvent<GoalSO> OnGoalCompleted;
        

        [Title("Goal System Debug")]
        [ShowInInspector, ReadOnly, ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        private List<GoalDebugInfo> goalDebugList = new List<GoalDebugInfo>();

        [Serializable]
        public class GoalDebugInfo
        {
            [HorizontalGroup("Header"), LabelWidth(100)]
            [ShowInInspector, ReadOnly] public string Name;

            [HorizontalGroup("Header"), LabelWidth(60)]
            [ShowInInspector, ReadOnly] public bool IsActive;

            [HorizontalGroup("Header"), LabelWidth(90)]
            [ShowInInspector, ReadOnly] public bool IsCompleted;

            [ShowInInspector, ReadOnly]
            [TableList(ShowIndexLabels = true, IsReadOnly = true)]
            public List<RequirementDebugInfo> Requirements;

            [ShowInInspector, ReadOnly]
            [ProgressBar(0, 1, ColorMember = "ProgressColor")]
            public float Progress;

            public Color ProgressColor => IsCompleted ? Color.green : (IsActive ? Color.yellow : Color.gray);
        }

        [Serializable]
        public class RequirementDebugInfo
        {
            [TableColumnWidth(150, false)]
            [ShowInInspector, ReadOnly] public string Name;

            [TableColumnWidth(100, false)]
            [ShowInInspector, ReadOnly] public string Type;

            [ShowInInspector, ReadOnly] public bool IsMet;

            [ShowInInspector, ReadOnly] public string CurrentValue;

            [ShowInInspector, ReadOnly] public string TargetValue;

            [ProgressBar(0, 1, ColorMember = "ProgressColor")]
            [ShowInInspector, ReadOnly] public float Progress;

            public Color ProgressColor => IsMet ? Color.green : Color.yellow;
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                InitializePlayMode();
            }
            else
            {
                InitializeEditMode();
            }
        }

        private void InitializePlayMode()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeEditMode()
        {
            _instance = this;
        }

        private void OnEnable()
        {
            _instance = this;
        }

        private void Update()
        {
            UpdateDebugInfo();
        }
        public void AddGoal(GoalSO goalSO)
        {
            if (!goalDataMap.ContainsKey(goalSO))
            {
                goalDataMap[goalSO] = new GoalData(goalSO);
                Log($"Goal Added", goalSO.goalName);
                UpdateDebugInfo();
            }
            else
            {
                Log($"Goal already tracked", goalSO.goalName, true);
            }
        }

        public void RemoveGoal(GoalSO goalSO)
        {
            if (goalDataMap.Remove(goalSO))
            {
                Log($"Goal Removed", goalSO.goalName);
                UpdateDebugInfo();
            }
            else
            {
                Log($"Goal Not Tracked", goalSO.goalName, true);
            }
        }

        public void ActivateGoal(GoalSO goalSO)
        {
            if (goalDataMap.TryGetValue(goalSO, out GoalData goalData) && !goalData.IsActive)
            {
                goalData.IsActive = true;
                OnGoalActivated.Invoke(goalSO);
                Log($"Goal activated", goalSO.goalName);
                UpdateDebugInfo();
            }
        }
        
        public void ResetGoal(GoalSO goalSO)
        {
            if (goalDataMap.TryGetValue(goalSO, out GoalData goalData) && !goalData.IsActive)
            {
                foreach (RequirementData requirement in goalData.RequirementDatas)
                {
                    requirement.Reset();
                }

                goalData.IsActive = true;
                OnGoalActivated.Invoke(goalSO);
                Log($"Goal activated", goalSO.goalName);
                UpdateDebugInfo();
            }
        }
        
        public bool IsGoalActive(GoalSO goalSO)
        {
            return goalDataMap.TryGetValue(goalSO, out GoalData goalData) && goalData.IsActive;
        }

        public bool IsGoalCompleted(GoalSO goalSO)
        {
            return goalDataMap.TryGetValue(goalSO, out GoalData goalData) && goalData.IsCompleted();
        }

        public List<GoalSO> GetActiveGoals()
        {
            return goalDataMap.Where(kvp => kvp.Value.IsActive).Select(kvp => kvp.Key).ToList();
        }

        public List<GoalSO> GetCompletedGoals()
        {
            return goalDataMap.Where(kvp => kvp.Value.IsCompleted()).Select(kvp => kvp.Key).ToList();
        }

        private void UpdateGoal(GoalSO goalSO)
        {
            bool gotData = goalDataMap.TryGetValue(goalSO, out GoalData goalData);
            Log("UPDATING GOAL", goalSO.goalName + " | " + gotData);
            if (gotData && goalData.IsActive && goalData.IsCompleted())
            {
                OnGoalCompleted.Invoke(goalSO);
                Log($"Goal complete", goalSO.goalName);
                UpdateDebugInfo();
            }
        }
        public void SetBoolRequirement(BoolRequirementSO requirementSO, bool value)
        {
            foreach (var goalData in goalDataMap.Values)
            {
                var reqData = goalData.RequirementDatas.Find(r => r.Config == requirementSO) as BoolRequirementData;
                if (reqData != null)
                {
                    reqData.value = value;
                    UpdateGoal(goalData.Config);
                }
            }
            UpdateDebugInfo();
        }

        public void IncrementIntRequirement(IntRequirementSO requirementSO, int amount)
        {
            foreach (var goalData in goalDataMap.Values)
            {
                if(!goalData.IsActive) return;
                
                var reqData = goalData.RequirementDatas.Find(r => r.Config == requirementSO) as IntRequirementData;
                if (reqData != null)
                {
                    reqData.currentValue += amount;
                    UpdateGoal(goalData.Config);
                }
            }
            UpdateDebugInfo();
        }

        private void UpdateDebugInfo()
        {
            goalDebugList = GetGoalDebugList();
        }
        public List<GoalDebugInfo> GetGoalDebugList()
        {
            return goalDataMap.Select(kvp => new GoalDebugInfo
            {
                Name = kvp.Key.goalName,
                IsActive = kvp.Value.IsActive,
                IsCompleted = kvp.Value.IsCompleted(),
                Requirements = kvp.Value.RequirementDatas.Select(r => new RequirementDebugInfo
                {
                    Name = r.Config.requirementName,
                    Type = r.GetType().Name,
                    IsMet = r.IsMet(),
                    CurrentValue = GetRequirementCurrentValue(r),
                    TargetValue = GetRequirementTargetValue(r),
                    Progress = GetRequirementProgress(r)
                }).ToList(),
                Progress = kvp.Value.RequirementDatas.Count > 0 
                    ? (float)kvp.Value.RequirementDatas.Count(r => r.IsMet()) / kvp.Value.RequirementDatas.Count 
                    : 0f
            }).ToList();
        }

        private string GetRequirementCurrentValue(RequirementData r)
        {
            return r switch
            {
                BoolRequirementData boolReq => boolReq.value.ToString(),
                IntRequirementData intReq => intReq.currentValue.ToString(),
                _ => "N/A"
            };
        }

        private string GetRequirementTargetValue(RequirementData r)
        {
            return r switch
            {
                BoolRequirementData => "True",
                IntRequirementData intReq => ((IntRequirementSO)intReq.Config).targetValue.ToString(),
                _ => "N/A"
            };
        }

        private float GetRequirementProgress(RequirementData r)
        {
            return r switch
            {
                BoolRequirementData boolReq => boolReq.value ? 1f : 0f,
                IntRequirementData intReq => Mathf.Clamp01((float)intReq.currentValue / ((IntRequirementSO)intReq.Config).targetValue),
                _ => 0f
            };
        }
    }
}