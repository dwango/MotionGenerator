using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ShoutingLoveDifferencialSoulSaveData : ISoulSaveData, IALifeSerializable<ShoutingLoveDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ShoutingLoveDifferencialSoul();
        }
    }
}