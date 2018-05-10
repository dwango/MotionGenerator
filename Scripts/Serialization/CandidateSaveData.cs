using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class CandidateSaveData : IALifeSerializable<CandidateSaveData>
    {
        [Key(0)] public float Mean { get; set; }
        [Key(1)] public float MeanSquare { get; set; }
        [Key(2)] public float Variance { get; set; }
        [Key(3)] public float Std { get; set; }
        [Key(4)] public float NumTried { get; set; }
        [Key(5)] public MotionSequenceSaveData[] Value { get; set; }

        public CandidateSaveData()
        {
            
        }

        public CandidateSaveData(float mean, float meanSquare, float variance, float std, float numTried,
            MotionSequenceSaveData[] value)
        {
            Mean = mean;
            MeanSquare = meanSquare;
            Variance = variance;
            Std = std;
            NumTried = numTried;
            Value = value;
        }
    }
}