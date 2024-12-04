using START.Scripts.GoalSystem;
using START.scripts.GoalSystem.ScriptableObjects;
using UnityEngine;

namespace START.scripts.GoalSystem
{
    public class GoalActivator : MonoBehaviour
    {
        [SerializeField] private GoalSO goal;

        public void ActivateGoal()
        {
            if(goal == null) return;
            GoalManager.Instance.ActivateGoal(goal, true);
        }
    }
}
