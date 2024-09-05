using START.Scripts.GoalSystem;

namespace START.scripts.GoalSystem.ScriptableObjects
{
    public class ConversationRequirementData : RequirementData
    {
        public string currentValue;
        private string targetValue;

        public ConversationRequirementData(ConversationRequirementSO config)
        {
            Config = config;
            currentValue = config.defaultValue;
            targetValue = config.targetValue;
        }

        public override void Reset()
        {
            currentValue = "";
        }

        public override bool IsMet() => currentValue == targetValue;
    }
}
