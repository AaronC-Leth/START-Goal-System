using START.Scripts.GoalSystem;
using UnityEngine;

namespace START.scripts.GoalSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Conversation Requirement", menuName = "START/Requirements/Conversation Requirement")]
    public class ConversationRequirementSO : RequirementSO
    {
        public string defaultValue;
        public string targetValue;

        public override RequirementData CreateRuntimeData() => new ConversationRequirementData(this);
    }
}
