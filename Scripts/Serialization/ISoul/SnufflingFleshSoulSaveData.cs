using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingFleshSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<SnufflingFleshSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingFleshSoul();
        }
    }
}