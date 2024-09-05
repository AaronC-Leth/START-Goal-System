using System.Collections.Generic;

namespace START.scripts.GoalSystem.ScriptableObjects
{
    public class GoalData
    {
        public GoalSO Config { get; private set; }
        public bool IsActive { get; set; }
        public List<RequirementData> RequirementDatas { get; private set; }

        public GoalData(GoalSO config)
        {
            Config = config;
            IsActive = false;
            RequirementDatas = new List<RequirementData>();
            foreach (var req in config.requirements)
            {
                RequirementDatas.Add(req.CreateRuntimeData());
            }
        }

        public bool IsCompleted()
        {
            return RequirementDatas.TrueForAll(r => r.IsMet());
        }
    }
}