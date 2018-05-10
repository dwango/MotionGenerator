using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ShoutingLoveSoulSaveData : ISoulSaveData, IALifeSerializable<ShoutingLoveSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ShoutingLoveSoul();
        }
    }
}