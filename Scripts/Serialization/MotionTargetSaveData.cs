using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class MotionTargetSaveData : IALifeSerializable<MotionTargetSaveData>
    {
        [Key(0)] public float Time { get; set; }
        [Key(1)] public List<float> Value { get; set; }

        public MotionTargetSaveData()
        {
            
        }

        public MotionTargetSaveData(float time, List<float> value)
        {
            Time = time;
            Value = value;
        }
    }
}