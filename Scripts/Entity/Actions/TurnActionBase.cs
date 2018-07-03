using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public abstract class TurnActionBase : ActionBase
    {
        public TurnActionBase(string name) : base(name)
        {
        }

        public TurnActionBase(TurnActionBase src) : base(src)
        {
        }

        public TurnActionBase(ActionBaseSaveData saveData) : base(saveData)
        {
        }

        protected static float GetRotatedAngle(State lastState, State nowState)
        {
            var lastRotation = lastState.GetAsQuaternion(State.BasicKeys.Rotation);
            var nowRotation = nowState.GetAsQuaternion(State.BasicKeys.Rotation);
            var relative = Quaternion.Inverse(lastRotation) * nowRotation;
            var angle = relative.eulerAngles[1]; // range: [0,360]
            // Move range to [-180, 180]
            if (angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }

        protected static float GetMovedDistance(State lastState, State nowState)
        {
            var lastPosition = lastState.GetAsVector3(State.BasicKeys.Position);
            var currentPosition = nowState.GetAsVector3(State.BasicKeys.Position);
            return (lastPosition - currentPosition).magnitude;
        }

        protected static float GetActionTime(State lastState, State nowState)
        {
            var lastTime = lastState.GetAsDouble(State.BasicKeys.Time);
            var currentTime = nowState.GetAsDouble(State.BasicKeys.Time);
            return (float) (currentTime - lastTime);
        }

        protected static float GetManipulatorEnergyConsumption(State nowState)
        {
            return (float) nowState[State.BasicKeys.ManipulatorEnergyConsumption][0];
        }
    }
}