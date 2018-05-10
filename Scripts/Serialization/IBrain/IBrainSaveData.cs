using MessagePack;

namespace MotionGenerator.Serialization
{
    [Union(0, typeof(BrainSaveData))]
    public interface IBrainSaveData
    {
        IBrain Instantiate();
    }
}