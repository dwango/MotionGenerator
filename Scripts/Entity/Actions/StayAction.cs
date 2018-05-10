using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class StayAction : TurnActionBase
    {
        public StayAction(string name) : base(name)
        {
        }

        public StayAction() : this("stay")
        {
        }

        public StayAction(StayAction src) : base(src)
        {
        }

        public StayAction(StayActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new StayActionSaveData Save()
        {
            return new StayActionSaveData(
                base.Save()
            );
        }

        public override IActionSaveData SaveAsInterface()
        {
            return Save();
        }

        public override IAction Clone()
        {
            return new StayAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            var rotatedAngle = GetRotatedAngle(lastState, nowState);
            var movedDistance = GetMovedDistance(lastState, nowState);
            var actionTime = GetActionTime(lastState, nowState);

            return -(Mathf.Abs(rotatedAngle) / 180f + movedDistance) / actionTime;
        }
    }
}