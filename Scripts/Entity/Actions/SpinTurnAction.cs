using System;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class SpinTurnAction : TurnActionBase
    {
        public static SpinTurnAction TurnLeftAction(string name = "spinTurnLeft", float acceptableMovement = 1f)
        {
            return new SpinTurnAction(name, 45f, acceptableMovement);
        }

        public static SpinTurnAction TurnRightAction(string name = "spinTurnRight", float acceptableMovement = 1f)
        {
            return new SpinTurnAction(name, -45f, acceptableMovement);
        }

        public readonly float TargetAngle;
        public readonly float AcceptableMovement;

        public SpinTurnAction(string name, float targetAngle, float acceptableMovement) : base(name)
        {
            TargetAngle = targetAngle;
            AcceptableMovement = acceptableMovement;
        }

        public SpinTurnAction(SpinTurnAction src) : base(src)
        {
            TargetAngle = src.TargetAngle;
            AcceptableMovement = src.AcceptableMovement;
        }

        public SpinTurnAction(SpinTurnActionSaveData saveData)
            : base(saveData.ActionBase)
        {
            TargetAngle = saveData.TargetAngle;
            AcceptableMovement = saveData.AcceptableMovement;
        }

        public new SpinTurnActionSaveData Save()
        {
            return new SpinTurnActionSaveData(
                base.Save(),
                TargetAngle,
                AcceptableMovement
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new SpinTurnAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            var rotatedAngle = GetRotatedAngle(lastState, nowState);
            var movedDistance = GetMovedDistance(lastState, nowState);

            if (movedDistance <= AcceptableMovement)
            {
                movedDistance = AcceptableMovement;
            }
            return (180f - Mathf.Abs(rotatedAngle + TargetAngle)) / movedDistance;
        }
    }
}