namespace START.GoalSystem.ScriptableObjects
{
    public abstract class RequirementData
    {
        public RequirementSO Config { get; protected set; }
        public abstract void Reset();
        public abstract bool IsMet();
    }
}