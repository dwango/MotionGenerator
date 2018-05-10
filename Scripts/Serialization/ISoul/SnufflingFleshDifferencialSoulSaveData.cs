using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingFleshDifferencialSoulSaveData : ISoulSaveData,
        IALifeSerializable<SnufflingFleshDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingFleshDifferencialSoul();
        }
    }

    [MessagePackObject]
    public sealed class ModerateSnufflingFleshDifferencialSoulSaveData : ISoulSaveData,
        IALifeSerializable<ModerateSnufflingFleshDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateSnufflingFleshDifferencialSoul();
        }
    }
}