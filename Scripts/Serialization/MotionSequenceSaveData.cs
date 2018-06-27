using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class MotionSequenceSaveData : IMotionGeneratorSerializable<MotionSequenceSaveData>
    {
        [Key(0)] public List<MotionTargetSaveData> Sequence { get; set; }

        public MotionSequenceSaveData()
        {
            
        }

        public MotionSequenceSaveData(List<MotionTargetSaveData> sequence)
        {
            Sequence = sequence;
        }
    }
}