using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingDifferencialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<SnufflingDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingDifferencialSoul();
        }
    }

    [MessagePackObject]
    public sealed class ModerateSnufflingDifferencialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<ModerateSnufflingDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateSnufflingDifferencialSoul();
        }
    }
}