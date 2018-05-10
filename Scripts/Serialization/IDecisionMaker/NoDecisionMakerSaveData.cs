using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class NoDecisionMakerSaveData : IDecisionMakerSaveData, IALifeSerializable<NoDecisionMakerSaveData>
    {
        [Key(0)] public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }

        public NoDecisionMakerSaveData()
        {
            
        }

        public NoDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase)
        {
            DecisionMakerBase = decisionMakerBase;
        }

        public IDecisionMaker Instantiate()
        {
            return new NoDecisionMaker(this);
        }
    }
}