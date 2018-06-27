using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class CowardDiffrencialSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<CowardDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CowardDiffrencialSoul();
        }
    }
}