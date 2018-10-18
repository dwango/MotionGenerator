using System;
using System.Collections.Generic;
using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class FixedSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<FixedSequenceMakerSaveData>
    {
        [Key(0)] public Dictionary<string, Dictionary<Guid, MotionSequenceSaveData>> MotionDict { get; set; }

        public FixedSequenceMakerSaveData()
        {
            
        }

        public FixedSequenceMakerSaveData(Dictionary<string, Dictionary<Guid, MotionSequenceSaveData>> motionDict)
        {
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