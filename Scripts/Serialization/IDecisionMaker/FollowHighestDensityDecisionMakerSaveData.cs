using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class FollowHighestDensityDecisionMakerSaveData : IDecisionMakerSaveData,
        IALifeSerializable<FollowHighestDensityDecisionMakerSaveData>
    {
        [Key(0)]
        public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }

        [Key(1)]
        public string StateKey { get; set; }

        [Key(2)]
        public bool IsNegative { get; set; }

        public FollowHighestDensityDecisionMakerSaveData()
        {
        }

        public FollowHighestDensityDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase, string stateKey,
            bool isNegative)
        {
            DecisionMakerBase = decisionMakerBase;
            StateKey = stateKey;
            IsNegative = isNegative;
        }

        public IDecisionMaker Instantiate()
        {
            return new FollowHighestDensityDecisionMaker(this);
        }
    }
}