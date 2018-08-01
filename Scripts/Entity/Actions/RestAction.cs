using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class RestAction : TurnActionBase
    {
        public RestAction(string name) : base(name)
        {
        }

        public RestAction() : this("rest")
        {
        }

        public RestAction(RestAction src) : base(src)
        {
        }

        public RestAction(RestActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new RestActionSaveData Save()
        {
            return new RestActionSaveData(
                base.Save()
            );
        }

        public override IActionSaveData SaveAsInterface()
        {
            return Save();
        }

        public override IAction Clone()
        {
            return new RestAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            var rotatedAngle = GetRotatedAngle(lastState, nowState);
            var movedDistance = GetMovedDistance(lastState, nowState);
            var actionTime = GetActionTime(lastState, nowState);
            var manipulatorEnergyConsumption = GetAverageManipulatorEnergyConsumption(nowState);

            return -(Mathf.Abs(rotatedAngle) / 180f + movedDistance) / actionTime * manipulatorEnergyConsumption;
        }
    }
}