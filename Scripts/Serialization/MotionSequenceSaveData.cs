using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class MotionSequenceSaveData : IALifeSerializable<MotionSequenceSaveData>
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