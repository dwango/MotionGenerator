using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class Candidate
    {
        public float mean;
        private float _meanSquare; // mean of square
        public float variance;
        public float std;
        public float numTried;
        public List<MotionSequence> value;

        public Candidate(List<MotionSequence> value)
        {
            this.value = value;
            numTried = 0;
            mean = 0;
            _meanSquare = 0;
            std = float.MaxValue;
            variance = float.MaxValue; // to be choosen
        }

        public Candidate(Candidate other) // deep copy
        {
            mean = other.mean;
            _meanSquare = other._meanSquare;
            variance = other.variance;
            std = other.std;
            numTried = other.numTried;
            value = other.value.ToList();
        }

        public Candidate(CandidateSaveData saveData)
        {
            mean = saveData.Mean;
            _meanSquare = saveData.MeanSquare;
            variance = saveData.Variance;
            std = saveData.Std;
            numTried = saveData.NumTried;
            value = saveData.Value.Select(x => new MotionSequence(x)).ToList();
        }

        public CandidateSaveData Save()
        {
            return new CandidateSaveData(
                mean,
                _meanSquare,
                variance,
                std,
                numTried,
                value.Select(x => x.Save()).ToArray()
            );
        }

        public void Update(float reward)
        {
            // online update of mean and std
            numTried += 1;
            mean += (reward - mean) / numTried;
            _meanSquare += (reward * reward - _meanSquare) / numTried;
            variance = _meanSquare - mean * mean;
            std = (float) Math.Sqrt(variance);

            if (numTried <= 1)
            {
                std = float.MaxValue;
                variance = float.MaxValue;
            }
        }

        public bool IsCompletelyBetterThan(Candidate another, float minimumProbability)
        {
            if (variance == 0)
                return false;
            if (minimumProbability == 0)
                return false;

            // if minimumProbability == 0.05f, criticalValue == 1.644854
            // if minimumProbability == 0.01f,  criticalValue == 2.326348
            var criticalValue = (float) Normal.InvCDF(0, 1, 1f - minimumProbability);
            var z = (mean - another.mean) / Math.Sqrt(variance / numTried + another.variance / another.numTried);
            return z >= criticalValue;
        }

        public override string ToString()
        {
            return string.Format("numTried:{0}, mean:{0}, variance:{0}", numTried, mean, variance);
        }
    }
}