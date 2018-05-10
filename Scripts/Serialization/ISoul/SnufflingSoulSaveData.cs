using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingSoulSaveData : ISoulSaveData, IALifeSerializable<SnufflingSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingSoul();
        }
    }
}