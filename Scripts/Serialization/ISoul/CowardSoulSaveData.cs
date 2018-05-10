using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class CowardSoulSaveData : ISoulSaveData, IALifeSerializable<CowardSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new CowardSoul();
        }
    }
}