using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class RandomSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<RandomSequenceMakerSaveData>
    {
        [Key(0)] public SequenceMakerBaseSaveData SequenceMakerBase { get; set; }
        [Key(1)] public float TimeRange { get; set; }
        [Key(2)] public float ValueRange { get; set; }
        [Key(3)] public int NumControlPoints { get; set; }
        [Key(4)] public List<int> OutputDimentions { get; set; }

        public RandomSequenceMakerSaveData()
        {
            
        }

        public RandomSequenceMakerSaveData(SequenceMakerBaseSaveData sequenceMakerBase, float timeRange,
            float valueRange, int numControlPoints, List<int> outputDimentions)
        {
            SequenceMakerBase = sequenceMakerBase;
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