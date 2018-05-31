using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class LazySoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<LazySoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new LazySoul();
        }
    }
}