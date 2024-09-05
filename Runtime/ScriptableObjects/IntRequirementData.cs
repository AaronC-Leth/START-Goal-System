namespace START.GoalSystem.ScriptableObjects
{
    public class IntRequirementData : RequirementData
    {
        public int currentValue;
        private int targetValue;

        public IntRequirementData(IntRequirementSO config)
        {
            Config = config;
            currentValue = config.defaultValue;
            targetValue = config.targetValue;
        }


        public int Value()
        {
            return currentValue;
        }
        
        public override void Reset()
        {
            currentValue = 0;
        }

        public override bool IsMet() => currentValue >= targetValue;
    }
}