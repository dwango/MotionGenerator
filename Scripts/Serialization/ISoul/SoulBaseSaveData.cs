using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SoulBaseSaveData : ISoulSaveData, IALifeSerializable<SoulBaseSaveData>
    {
        public ISoul Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}