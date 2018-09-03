using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public interface ISoul
    {
        float Reward(State lastState, State nowState);
        SoulSaveData SaveAsInterface();
    }
}