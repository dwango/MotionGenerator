using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class Candidate3D
    {
        public Vector3 Mean;
        public float AxisYAngularVelocity;
        public int NumTried;

        // TODO(nakamura): これだと中身の変更ができるので、System.Collections.ObjectModel.ReadOnlyDictionaryに変える
        public List<MotionSequence> Value { get; private set; }

        public Candidate3D(Candidate3D other)
        {
            Mean = other.Mean;
            AxisYAngularVelocity = other.AxisYAngularVelocity;
            NumTried = other.NumTried;
            Value = other.Value;
        }

        public Candidate3D(List<MotionSequence> value)
        {
            Value = value;
            NumTried = 0;
        }

        public Candidate3D(Candidate3DSaveData saveData)
        {
            Mean = saveData.Mean;
            AxisYAngularVelocity = saveData.AxisYAngularVelocity;
            NumTried = saveData.NumTried;
            Value = saveData.Value.Select(x => new MotionSequence(x)).ToList();
        }


        public Candidate3DSaveData Save()
        {
            return new Candidate3DSaveData(
                Mean,
                NumTried,
                AxisYAngularVelocity,
                Value.Select(x => x.Save()).ToList()
            );
        }

        public void Update(Vector3 v, float axisYAngularVelocity)
        {
            // to 2D
            v = new Vector3(v.x, 0, v.z);

            NumTried += 1;
            Mean += (v - Mean) / (float) NumTried;
            AxisYAngularVelocity += (axisYAngularVelocity - AxisYAngularVelocity) / (float) NumTried;
        }
    }
}
