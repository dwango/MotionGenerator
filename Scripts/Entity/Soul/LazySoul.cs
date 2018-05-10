using MotionGenerator.Serialization;
using UnityEngine.Assertions;

namespace MotionGenerator.Entity.Soul
{
    public class LazySoul : Soul
    {
        public override float Reward(State lastState, State nowState)
        {
            return -Distance(lastState[State.BasicKeys.Position], nowState[State.BasicKeys.Position]);
        }

        public override ISoulSaveData SaveAsInterface()
        {
            return new LazySoulSaveData();
        }
    }
}