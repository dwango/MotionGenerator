using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class HeuristicReinforcementDecisionMakerSaveData : IDecisionMakerSaveData,
        IMotionGeneratorSerializable<HeuristicReinforcementDecisionMakerSaveData>
    {
        [Key(0)]
        public ReinforcementDecisionMakerSaveData DecisionMakerBase { get; set; }

        [Key(1)]
        public int PracticeDecisionMakerIndex { get; set; }

        [Key(2)]
        public int PracticeDecisionCount { get; set; }

        [Key(3)]
        public int EmergencyDecisionMakerIndex { get; set; }

        [Key(4)]
        public float EmergencyEnergyRatio { get; set; }

        public HeuristicReinforcementDecisionMakerSaveData()
        {
        }

        public HeuristicReinforcementDecisionMakerSaveData(ReinforcementDecisionMakerSaveData decisionMakerBase,
            int practiceDecisionMakerIndex, int practiceDecisionCount,
            int emergencyDecisionMakerIndex, float emergencyEnergyRatio)
        {
            DecisionMakerBase = decisionMakerBase;
            PracticeDecisionMakerIndex = practiceDecisionMakerIndex;
            PracticeDecisionCount = practiceDecisionCount;
            EmergencyDecisionMakerIndex = emergencyDecisionMakerIndex;
            EmergencyEnergyRatio = emergencyEnergyRatio;
        }

        public IDecisionMaker Instantiate()
        {
            return new HeuristicReinforcementDecisionMaker(this);
        }
    }
}