using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class RemoteDecisionMakerSaveData : IDecisionMakerSaveData, IMotionGeneratorSerializable<RemoteDecisionMakerSaveData>
    {
        [Key(0)] public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }
        [Key(1)] public string RemoteId { get; set; }

        public RemoteDecisionMakerSaveData()
        {
            
        }

        public RemoteDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase, string remoteId)
        {
            DecisionMakerBase = decisionMakerBase;
            RemoteId = remoteId;
        }

        public IDecisionMaker Instantiate()
        {
            return new RemoteDecisionMaker(this);
        }
    }
}