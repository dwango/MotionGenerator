using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class BumpingDifferenceialSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<BumpingDifferenceialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new BumpingDifferencialSoul();
        }
    }

    [MessagePackObject]
    public class ModerateBumpingDifferenceialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<ModerateBumpingDifferenceialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateBumpingDifferencialSoul();
        }
    }
    
    [MessagePackObject]
    public class AvoidObjectSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<AvoidObjectSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new AvoidObjectSoul();
        }
    }
}