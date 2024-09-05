using UnityEngine;
using System.Collections.Generic;

namespace START.GoalSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Goal", menuName = "START/Goal")]
    public class GoalSO : ScriptableObject
    {
        public string goalName;
        public List<RequirementSO> requirements;
    }
}