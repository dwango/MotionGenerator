using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class BumpingDifferenceialSoulSaveData : ISoulSaveData, IALifeSerializable<BumpingDifferenceialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new BumpingDifferencialSoul();
        }
    }

    [MessagePackObject]
    public class ModerateBumpingDifferenceialSoulSaveData : ISoulSaveData,
        IALifeSerializable<ModerateBumpingDifferenceialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateBumpingDifferencialSoul();
        }
    }
    
    [MessagePackObject]
    public class AvoidObjectSoulSaveData : ISoulSaveData,
        IALifeSerializable<AvoidObjectSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new AvoidObjectSoul();
        }
    }
}