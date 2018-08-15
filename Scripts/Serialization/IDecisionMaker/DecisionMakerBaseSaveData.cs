using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class DecisionMakerBaseSaveData : IDecisionMakerSaveData, IMotionGeneratorSerializable<DecisionMakerBaseSaveData>
    {
        [Key(0)] public List<ActionSaveData> Actions { get; set; }

        public DecisionMakerBaseSaveData()
        {
            
        }

        public DecisionMakerBaseSaveData(List<ActionSaveData> actions)
        {
            Actions = actions;
        }

        public IDecisionMaker Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}