using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class DecisionMakerBaseSaveData : IDecisionMakerSaveData, IMotionGeneratorSerializable<DecisionMakerBaseSaveData>
    {
        [Key(0)] public List<IActionSaveData> Actions { get; set; }

        public DecisionMakerBaseSaveData()
        {
            
        }

        public DecisionMakerBaseSaveData(List<IActionSaveData> actions)
        {
            Actions = actions;
        }

        public IDecisionMaker Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}