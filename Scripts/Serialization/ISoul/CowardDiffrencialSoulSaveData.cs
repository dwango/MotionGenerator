using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class CowardDiffrencialSoulSaveData : ISoulSaveData, IALifeSerializable<CowardDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CowardDiffrencialSoul();
        }
    }
}