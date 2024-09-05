using START.GoalSystem;
using START.GoalSystem.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace START.GoalSystem
{
    public class RequirementTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private IntRequirementSO intRequirement;
        [SerializeField] private string textFormatString = "You have {value} items.";

        private int currentValue;

        private void Start()
        {
            if (textComponent == null)
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }

            if (textComponent == null)
            {
                Debug.LogError("TextMeshProUGUI component not found!");
                return;
            }

            // Subscribe to the requirement update event
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.OnRequirementUpdated.AddListener(OnRequirementUpdated);
            }

            // Initial text update
            UpdateText();
        }

        private void OnDestroy()
        {
            // Unsubscribe from the requirement update event to avoid memory leaks
            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.OnRequirementUpdated.RemoveListener(OnRequirementUpdated);
            }
        }

        // Event handler for when a requirement is updated
        private void OnRequirementUpdated(RequirementData requirementData)
        {
            // Only update if the changed requirement matches the one this updater is monitoring
            if (requirementData.Config == intRequirement)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            if (GoalManager.Instance != null && intRequirement != null)
            {
                currentValue = GetCurrentRequirementValue();
                string formattedText = textFormatString.Replace("{value}", currentValue.ToString());
                textComponent.text = formattedText;
            }
        }

        private int GetCurrentRequirementValue()
        {
            var goals = GoalManager.Instance.GetActiveGoals();
            foreach (var goal in goals)
            {
                GoalData goalInfo = GoalManager.GetGoalByName(goal.goalName);
                if (goalInfo != null)
                {
                    IntRequirementData requirement = GoalManager.GetIntRequirement(goalInfo, intRequirement.requirementName);
                    if (requirement != null)
                    {
                        return requirement.currentValue;
                    }
                }
            }
            return 0;
        }
    }
}
