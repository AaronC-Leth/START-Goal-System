// File: RequirementUpdater.cs

using START.scripts.GoalSystem.ScriptableObjects;
using UnityEngine;

namespace START.Scripts.GoalSystem
{
    public class RequirementUpdater : MonoBehaviour
    {
        [System.Serializable]
        public class RequirementUpdate
        {
            public RequirementSO requirement;
            public UpdateType updateType;
            public int intValue = 1;
            public bool boolValue = true;
        }

        public enum UpdateType
        {
            SetBool,
            IncrementInt
        }

        public RequirementUpdate[] requirementUpdates;

        private void Start()
        {
            // Ensure GoalManager is available, especially for additively loaded scenes
            if (GoalManager.Instance == null)
            {
                Debug.LogError("GoalManager not found. Make sure it exists in the scene.");
                return;
            }
        }

        public void UpdateRequirements()
        {
            if (GoalManager.Instance == null)
            {
                Debug.LogError("GoalManager not found. Make sure it exists in the scene.");
                return;
            }

            foreach (var update in requirementUpdates)
            {
                switch (update.updateType)
                {
                    case UpdateType.SetBool:
                        if (update.requirement is BoolRequirementSO boolReq)
                        {
                            GoalManager.Instance.SetBoolRequirement(boolReq, update.boolValue);
                        }
                        else
                        {
                            Debug.LogError($"Requirement {update.requirement.name} is not a BoolRequirementSO");
                        }
                        break;
                    case UpdateType.IncrementInt:
                        if (update.requirement is IntRequirementSO intReq)
                        {
                            GoalManager.Instance.IncrementIntRequirement(intReq, update.intValue);
                        }
                        else
                        {
                            Debug.LogError($"Requirement {update.requirement.name} is not an IntRequirementSO");
                        }
                        break;
                }
            }
        }
    }
}