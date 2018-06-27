using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class StateSaveData : IMotionGeneratorSerializable<StateSaveData>
    {
        [Key(0)] public Dictionary<string, double[]> Values { get; set; }

        public StateSaveData()
        {
            
        }

        public StateSaveData(Dictionary<string, double[]> values)
        {
            Values = values;
        }
    }
}