using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingDifferencialSoulSaveData : ISoulSaveData,
        IALifeSerializable<SnufflingDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingDifferencialSoul();
        }
    }

    [MessagePackObject]
    public sealed class ModerateSnufflingDifferencialSoulSaveData : ISoulSaveData,
        IALifeSerializable<ModerateSnufflingDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateSnufflingDifferencialSoul();
        }
    }
}