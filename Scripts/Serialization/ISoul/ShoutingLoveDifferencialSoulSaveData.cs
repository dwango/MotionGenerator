using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ShoutingLoveDifferencialSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<ShoutingLoveDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ShoutingLoveDifferencialSoul();
        }
    }
}