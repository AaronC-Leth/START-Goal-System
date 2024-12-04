using START.Scripts.GoalSystem;

namespace START.scripts.GoalSystem.ScriptableObjects
{
    public class BoolRequirementData : RequirementData
    {
        public bool value;
        private bool defaultValue;

        public BoolRequirementData(BoolRequirementSO config)
        {
            Config = config;
            value = config.defaultValue;
            defaultValue = config.defaultValue;
        }

        public override void Reset()
        {
            value = defaultValue;
        }

        public override bool IsMet() => value;
    }
}