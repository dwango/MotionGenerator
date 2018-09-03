using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class BumpingDifferencialSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<BumpingDifferencialSoulSaveData>
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