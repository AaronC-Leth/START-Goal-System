using UnityEngine;

namespace START.scripts.GoalSystem.ScriptableObjects
{
    public abstract class RequirementSO : ScriptableObject
    {
        public string requirementName;
        public abstract RequirementData CreateRuntimeData();
    }
}