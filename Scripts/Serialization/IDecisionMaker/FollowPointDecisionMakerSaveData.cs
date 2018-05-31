using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class FollowPointDecisionMakerSaveData : IDecisionMakerSaveData,
        IMotionGeneratorSerializable<FollowPointDecisionMakerSaveData>
    {
        [Key(0)]
        public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }

        [Key(1)]
        public List<string> StateKeys { get; set; }

        [Key(2)]
        public bool IsNegative { get; set; }

        public FollowPointDecisionMakerSaveData()
        {
        }

        public FollowPointDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase, List<string> stateKeys,
            bool isNegative)
        {
            DecisionMakerBase = decisionMakerBase;
            StateKeys = stateKeys;
            IsNegative = isNegative;
        }

        public IDecisionMaker Instantiate()
        {
            return new FollowPointDecisionMaker(this);
        }
    }
}