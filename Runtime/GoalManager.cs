using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using START.GoalSystem.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace START.GoalSystem
{
    public class GoalManager : MonoBehaviour
    {
        private static GoalManager _instance;
        private static bool _isQuitting = false;
        
        #region START_Debug

        [SerializeField]
        private bool debugActive;

        [SerializeField]
        private Color debugColor = Color.cyan;
        
        [SerializeField]
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


        private Dictionary<GoalSO, GoalData> goalDataMap = new Dictionary<GoalSO, GoalData>();

        [SerializeField] // Make these serializable so they appear in the inspector
        public UnityEvent<GoalSO> OnGoalActivated;
        [SerializeField]
        public UnityEvent<GoalSO> OnGoalCompleted;
        
        [SerializeField]
        public UnityEvent<RequirementData> OnRequirementUpdated;
        
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


        public void AddGoal(GoalSO goalSO)
        {
            if (!goalDataMap.ContainsKey(goalSO))
            {
                goalDataMap[goalSO] = new GoalData(goalSO);
                Log($"Goal Added", goalSO.goalName);
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
            Log("UPDATING GOAL", goalSO.goalName);
            if (gotData && goalData.IsActive && goalData.IsCompleted())
            {
                OnGoalCompleted.Invoke(goalSO);
                Log($"Goal complete", goalSO.goalName);
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
                    OnRequirementUpdated.Invoke(reqData); // Trigger requirement update event
                    UpdateGoal(goalData.Config);
                }
            }
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
                    OnRequirementUpdated.Invoke(reqData); // Trigger requirement update event
                    UpdateGoal(goalData.Config);
                }
            }
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

        public static GoalData GetGoalByName(string goalName)
        {
            foreach (GoalData goal in Instance.goalDataMap.Values)
            {
                if (goal.Config.goalName == goalName)
                {
                    return goal;
                }
            }

            return null;
        }

        public static IntRequirementData GetIntRequirement(GoalData goalInfo, string intRequirementName)
        {
            if (goalInfo == null || intRequirementName == null)
            {
                return null;
            }

            foreach (RequirementData gData in goalInfo.RequirementDatas)
            {
                if (gData.GetType() == typeof(IntRequirementData))
                {
                    if (gData.Config.requirementName == intRequirementName)
                    {
                        IntRequirementData req = (IntRequirementData)gData;
                        return req;
                    }
                }
            }

            return null;
        }
    }
}