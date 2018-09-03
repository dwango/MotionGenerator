using MotionGenerator.Entity.Soul;

namespace MotionGenerator.Serialization
{
    public interface ISoulSaveData
    {
        ISoul Instantiate();
    }
}