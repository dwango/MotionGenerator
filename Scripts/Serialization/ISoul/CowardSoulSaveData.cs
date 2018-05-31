using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class CowardSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<CowardSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CowardSoul();
        }
    }
}