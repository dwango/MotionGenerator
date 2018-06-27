using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingFleshDifferencialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<SnufflingFleshDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingFleshDifferencialSoul();
        }
    }

    [MessagePackObject]
    public sealed class ModerateSnufflingFleshDifferencialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<ModerateSnufflingFleshDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateSnufflingFleshDifferencialSoul();
        }
    }
}