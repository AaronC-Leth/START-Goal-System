using System;
using System.Collections;
using System.Collections.Generic;
using START.scripts.GoalSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace START.Scripts.GoalSystem
{
    public class RequirementLevelManager : MonoBehaviour
    {
        [Serializable]
        public class GoalEntry
        {
            public bool activateByDefault;
            public GoalSO goal;
            public List<DelayedAction> activationEvents = new List<DelayedAction>();
            public List<DelayedAction> completionEvents = new List<DelayedAction>();

            // Custom property to get the goal name
            public string GoalName => goal != null ? goal.name : "Unnamed Goal";
        }

        [SerializeField]
        [Tooltip("List of goals managed by this level. Check 'Activate by Default' for goals that should be active when the level starts.")]
        private List<GoalEntry> goalList = new List<GoalEntry>();

        [Serializable]
        public class DelayedAction
        {
            public float delay;
            public UnityEvent action;
        }

        private Dictionary<GoalSO, Coroutine> activeCoroutines = new Dictionary<GoalSO, Coroutine>();

        public void Initialize()
        {
            InitializeAndActivateGoals();
            SubscribeToGoalEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromGoalEvents();
            RemoveGoals();
        }

        private void UnsubscribeFromGoalEvents()
        {
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.OnGoalActivated.RemoveListener(HandleGoalActivated);
                GoalManager.Instance.OnGoalCompleted.RemoveListener(HandleGoalCompleted);
            }
        }
        
        private void InitializeAndActivateGoals()
        {
            foreach (GoalEntry goalEntry in goalList)
            {
                if (goalEntry.goal == null)
                {
                    Debug.LogWarning("Null goal found in the goal list. Please check the RequirementLevelManager in the inspector.");
                    continue;
                }

                GoalManager.Instance.AddGoal(goalEntry.goal);

                if (goalEntry.activateByDefault)
                {
                    GoalManager.Instance.ActivateGoal(goalEntry.goal);
                }
            }
        }

        private void RemoveGoals()
        {
            if(GoalManager.Instance == null) return;
            foreach (GoalEntry goalEntry in goalList)
            {
                if (goalEntry.goal != null)
                {
                    GoalManager.Instance.RemoveGoal(goalEntry.goal);
                }
            }
        }

        private void SubscribeToGoalEvents()
        {
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.OnGoalActivated.AddListener(HandleGoalActivated);
                GoalManager.Instance.OnGoalCompleted.AddListener(HandleGoalCompleted);
            }
            else
            {
                Debug.LogWarning("GoalManager instance not found. Unable to subscribe to goal events.");
            }
        }

        public void ActivateGoalAtIndex(int index)
        {
            if(goalList == null || goalList.Count < 1) return;
            GoalManager.Instance.ActivateGoal(goalList[0].goal);
        }
        
        public void ActivateGoal(GoalSO goal)
        {
            if(goalList == null || goalList.Count < 1) return;
            GoalManager.Instance.ActivateGoal(goal);
        }
        
        private void HandleGoalActivated(GoalSO goal)
        {
            GoalEntry entry = goalList.Find(g => g.goal == goal);
            if (entry != null)
            {
                StopAndStartCoroutine(goal, entry.activationEvents);
            }
        }
        

        private void HandleGoalCompleted(GoalSO goal)
        {
            GoalEntry entry = goalList.Find(g => g.goal == goal);
            if (entry != null)
            {
                StopAndStartCoroutine(goal, entry.completionEvents);
            }
        }

        private void StopAndStartCoroutine(GoalSO goal, List<DelayedAction> actions)
        {
            if (activeCoroutines.TryGetValue(goal, out Coroutine coroutine))
            {
                if(coroutine != null) StopCoroutine(coroutine);
            }

            activeCoroutines[goal] = StartCoroutine(ExecuteDelayedActions(actions));
        }

        private IEnumerator ExecuteDelayedActions(List<DelayedAction> actions)
        {
            foreach (DelayedAction delayedAction in actions)
            {
                yield return new WaitForSeconds(delayedAction.delay);
                delayedAction.action.Invoke();
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(RequirementLevelManager))]
    public class RequirementLevelManagerEditor : Editor
    {
        private RequirementLevelManager manager;
        private List<bool> foldoutStates = new List<bool>();

        private void OnEnable()
        {
            manager = (RequirementLevelManager)target;
            InitializeFoldoutStates();
        }

        private void InitializeFoldoutStates()
        {
            SerializedProperty goalListProperty = serializedObject.FindProperty("goalList");
            foldoutStates.Clear();
            for (int i = 0; i < goalListProperty.arraySize; i++)
            {
                foldoutStates.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty goalListProperty = serializedObject.FindProperty("goalList");

            EditorGUILayout.LabelField("Goal List", EditorStyles.boldLabel);

            for (int i = 0; i < goalListProperty.arraySize; i++)
            {
                SerializedProperty goalEntryProperty = goalListProperty.GetArrayElementAtIndex(i);
                SerializedProperty goalProperty = goalEntryProperty.FindPropertyRelative("goal");
                SerializedProperty activateByDefaultProperty = goalEntryProperty.FindPropertyRelative("activateByDefault");

                string goalName = goalProperty.objectReferenceValue != null ? goalProperty.objectReferenceValue.name : "Unnamed Goal";
                string label = $"{i}: {goalName}";

                EditorGUILayout.BeginHorizontal();
                foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], label, true);

                // Add delete button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    goalListProperty.DeleteArrayElementAtIndex(i);
                    foldoutStates.RemoveAt(i);
                    break; // Exit the loop to avoid issues with modified array
                }

                EditorGUILayout.EndHorizontal();

                if (foldoutStates[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(activateByDefaultProperty);
                    EditorGUILayout.PropertyField(goalProperty);
                    EditorGUILayout.PropertyField(goalEntryProperty.FindPropertyRelative("activationEvents"));
                    EditorGUILayout.PropertyField(goalEntryProperty.FindPropertyRelative("completionEvents"));
                    EditorGUI.indentLevel--;
                }
            }

            if (GUILayout.Button("Add Goal Entry"))
            {
                goalListProperty.InsertArrayElementAtIndex(goalListProperty.arraySize);
                foldoutStates.Add(true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}