using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class CrowdDifferencialSoulSaveData: ISoulSaveData, IALifeSerializable<CrowdDifferencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CrowdDifferencialSoul();
        }
    }
}