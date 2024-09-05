using UnityEngine;

namespace START.GoalSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Int Requirement", menuName = "START/Requirements/Int Requirement")]
    public class IntRequirementSO : RequirementSO
    {
        public int defaultValue;
        public int targetValue;
        public override RequirementData CreateRuntimeData() => new IntRequirementData(this);
    }
}