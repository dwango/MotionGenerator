using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class NostalgiaSoulSaveData : ISoulSaveData, IALifeSerializable<NostalgiaSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new NostalgiaSoul();
        }
    }
}