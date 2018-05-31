using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SoulBaseSaveData : ISoulSaveData, IMotionGeneratorSerializable<SoulBaseSaveData>
    {
        public ISoul Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}