using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class CrowdDifferencialSoulSaveData: ISoulSaveData, IMotionGeneratorSerializable<CrowdDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CrowdDifferencialSoul();
        }
    }
}