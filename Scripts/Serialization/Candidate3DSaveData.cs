using System.Collections.Generic;
using MessagePack;
using Serialization;
using UnityEngine;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class Candidate3DSaveData : IALifeSerializable<Candidate3DSaveData>
    {
        [Key(0)] public Vector3 Mean { get; set; }
        [Key(1)] public int NumTried { get; set; }
        [Key(2)] public float AxisYAngularVelocity { get; set; }
        [Key(3)] public List<MotionSequenceSaveData> Value { get; set; }

        public Candidate3DSaveData()
        {
            
        }

        public Candidate3DSaveData(Vector3 mean, int numTried, float axisYAngularVelocity,
            List<MotionSequenceSaveData> value)
        {
            Mean = mean;
            NumTried = numTried;
            AxisYAngularVelocity = axisYAngularVelocity;
            Value = value;
        }
    }
}