using START.GoalSystem;
using UnityEngine;

namespace START.GoalSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Bool Requirement", menuName = "START/Requirements/Bool Requirement")]
    public class BoolRequirementSO : RequirementSO
    {
        public bool defaultValue;
        public override RequirementData CreateRuntimeData() => new BoolRequirementData(this);
    }
}