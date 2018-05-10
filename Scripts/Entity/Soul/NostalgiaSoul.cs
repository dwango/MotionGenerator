using MotionGenerator.Serialization;
using UnityEngine.Assertions;

namespace MotionGenerator.Entity.Soul
{
    public class NostalgiaSoul : Soul
    {
        public const float a = 1;
        public override float Reward(State lastState, State nowState)
        {
            Assert.IsTrue(lastState.ContainsKey(State.BasicKeys.BirthPosition));
            Assert.IsTrue(nowState.ContainsKey(State.BasicKeys.BirthPosition));
            Assert.IsTrue(lastState.ContainsKey(State.BasicKeys.Position));
            Assert.IsTrue(nowState.ContainsKey(State.BasicKeys.Position));

            var lastDistance = Distance(lastState[State.BasicKeys.BirthPosition], lastState[State.BasicKeys.Position]);
            var currentDistance = Distance(nowState[State.BasicKeys.BirthPosition], nowState[State.BasicKeys.Position]);
            return lastDistance - currentDistance;
        }

        public override ISoulSaveData SaveAsInterface()
        {
            return new NostalgiaSoulSaveData();
        }
    }
}