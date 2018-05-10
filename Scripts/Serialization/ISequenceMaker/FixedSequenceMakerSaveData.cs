using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class FixedSequenceMakerSaveData : ISequenceMakerSaveData, IALifeSerializable<FixedSequenceMakerSaveData>
    {
        [Key(0)] public SequenceMakerBaseSaveData SequenceMakerBase { get; set; }
        [Key(1)] public Dictionary<string, List<MotionSequenceSaveData>> MotionDict { get; set; }

        public FixedSequenceMakerSaveData()
        {
            
        }

        public FixedSequenceMakerSaveData(SequenceMakerBaseSaveData sequenceMakerBase,
            Dictionary<string, List<MotionSequenceSaveData>> motionDict)
        {
            SequenceMakerBase = sequenceMakerBase;
            MotionDict = motionDict;
        }

        public FixedSequenceMaker Instantiate()
        {
            return new FixedSequenceMaker(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}