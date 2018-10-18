using System;
using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class RandomSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<RandomSequenceMakerSaveData>
    {
        [Key(0)] public float TimeRange { get; set; }
        [Key(1)] public float ValueRange { get; set; }
        [Key(2)] public int NumControlPoints { get; set; }
        [Key(3)] public Dictionary<Guid, int> OutputDimentions { get; set; }

        public RandomSequenceMakerSaveData()
        {
            
        }

        public RandomSequenceMakerSaveData(float timeRange,
            float valueRange, int numControlPoints, Dictionary<Guid, int> outputDimentions)
        {
            TimeRange = timeRange;
            ValueRange = valueRange;
            NumControlPoints = numControlPoints;
            OutputDimentions = outputDimentions;
        }

        public RandomSequenceMaker Instantiate()
        {
            return new RandomSequenceMaker(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}