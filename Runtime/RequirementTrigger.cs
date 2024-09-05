using START.scripts.GoalSystem.ScriptableObjects;
using UnityEngine;

namespace START.Scripts.GoalSystem
{
    [RequireComponent(typeof(Collider))]
    public class RequirementTrigger : RequirementUpdater
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private bool debugMode = false;

        private bool hasTriggered = false;

        private void Start()
        {
            // Ensure the attached collider is a trigger
            Collider collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                Debug.LogWarning("Collider is not set as a trigger. Setting it to trigger mode.");
                collider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered && triggerOnce) return;

            if (other.CompareTag(targetTag))
            {
                UpdateRequirements();
                hasTriggered = true;

                if (debugMode)
                {
                    Debug.Log($"RequirementTrigger activated by {other.name} with tag {targetTag}");
                }
            }
        }

        // Override the UpdateRequirements method to add debug logging
        public new void UpdateRequirements()
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
                            if (debugMode) Debug.Log($"Set bool requirement {boolReq.name} to {update.boolValue}");
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
                            if (debugMode) Debug.Log($"Incremented int requirement {intReq.name} by {update.intValue}");
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