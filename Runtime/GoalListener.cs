using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using START.scripts.GoalSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace START.Scripts.GoalSystem
{
    public class GoalListener : MonoBehaviour
    {
        [SerializeField] private GoalSO targetGoal;

        [SerializeField, ListDrawerSettings(ShowIndexLabels = true, AddCopiesLastElement = true)]
        private List<DelayedAction> activationDelayedActions = new List<DelayedAction>();

        [SerializeField, ListDrawerSettings(ShowIndexLabels = true, AddCopiesLastElement = true)]
        private List<DelayedAction> completionDelayedActions = new List<DelayedAction>();

        private Coroutine activationActionSequenceCoroutine;
        private Coroutine completionActionSequenceCoroutine;

        [Serializable]
        public class DelayedAction
        {
            public float delay;
            public UnityEvent action;
        }

        private void OnDisable()
        {
            UnsubscribeFromGoalEvents();
        }

        public void SubscribeToGoalEvents()
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

        private void UnsubscribeFromGoalEvents()
        {
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.OnGoalActivated.RemoveListener(HandleGoalActivated);
                GoalManager.Instance.OnGoalCompleted.RemoveListener(HandleGoalCompleted);
            }
        }

        private void HandleGoalActivated(GoalSO goal)
        {
            if (goal == targetGoal)
            {
                StopAndStartCoroutine(ref activationActionSequenceCoroutine, activationDelayedActions);
            }
        }

        private void HandleGoalCompleted(GoalSO goal)
        {
            if (goal == targetGoal)
            {
                StopAndStartCoroutine(ref completionActionSequenceCoroutine, completionDelayedActions);
            }
        }

        private void StopAndStartCoroutine(ref Coroutine coroutine, List<DelayedAction> actions)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(ExecuteDelayedActions(actions));
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
}