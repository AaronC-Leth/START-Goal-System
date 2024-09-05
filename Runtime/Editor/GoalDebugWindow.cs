using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using START.Scripts.GoalSystem;

namespace START.Editor
{
    public class GoalDebugWindow : OdinEditorWindow
    {
        [MenuItem("Tools/START/Goal Debug")]
        private static void OpenWindow()
        {
            GetWindow<GoalDebugWindow>("Goal Debug").Show();
        }

        [ShowInInspector, ReadOnly, ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        private List<GoalManager.GoalDebugInfo> goalDebugList;

        private void OnEnable()
        {
            RefreshGoalDebugList();
        }

        [Button("Refresh")]
        private void RefreshGoalDebugList()
        {
            if (GoalManager.Instance != null)
            {
                goalDebugList = GoalManager.Instance.GetGoalDebugList();
            }
            else
            {
                goalDebugList = new List<GoalManager.GoalDebugInfo>();
            }
        }
    }
}