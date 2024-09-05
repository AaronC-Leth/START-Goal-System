using Sirenix.OdinInspector;
using START.Scripts.GoalSystem;
using START.scripts.GoalSystem.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace START.scripts.GoalSystem
{
    public class RequirementTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private IntRequirementSO intRequirement;
        [SerializeField] private string textFormatString = "You have {value} potions.";

        [SerializeField, ReadOnly] private int currentValue;

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

            UpdateText();
        }

        private void Update()
        {
            UpdateText();
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
                var goalDebugInfo = GoalManager.Instance.GetGoalDebugList().Find(g => g.Name == goal.goalName);
                if (goalDebugInfo != null)
                {
                    var requirement = goalDebugInfo.Requirements.Find(r => r.Name == intRequirement.requirementName);
                    if (requirement != null)
                    {
                        return int.Parse(requirement.CurrentValue);
                    }
                }
            }
            return 0;
        }
    }
}