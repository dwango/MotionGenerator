using System;
using System.Collections.Generic;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class LocomotionAction : ActionBase
    {
        public static List<IAction> EightDirections()
        {
            return new List<IAction>
            {
                GoStraight(),
                GoForwardRight(),
                GoRight(),
                GoBackRight(),
                GoBack(),
                GoBackLeft(),
                GoLeft(),
                GoForwardLeft(),
            };
        }

        public static LocomotionAction GoStraight(string name = "forward")
        {
            return new LocomotionAction(name, new Vector3(0, 0, 1));
        }

        public static LocomotionAction GoForwardRight(string name = "forwardRight")
        {
            return new LocomotionAction(name, new Vector3(1, 0, 1));
        }

        public static LocomotionAction GoRight(string name = "right")
        {
            return new LocomotionAction(name, new Vector3(1, 0, 0));
        }

        public static LocomotionAction GoBackRight(string name = "backRight")
        {
            return new LocomotionAction(name, new Vector3(1, 0, -1));
        }

        public static LocomotionAction GoBack(string name = "back")
        {
            return new LocomotionAction(name, new Vector3(0, 0, -1));
        }

        public static LocomotionAction GoBackLeft(string name = "backLeft")
        {
            return new LocomotionAction(name, new Vector3(-1, 0, -1));
        }

        public static LocomotionAction GoLeft(string name = "left")
        {
            return new LocomotionAction(name, new Vector3(-1, 0, 0));
        }

        public static LocomotionAction GoForwardLeft(string name = "forwardLeft")
        {
            return new LocomotionAction(name, new Vector3(-1, 0, 1));
        }

        public readonly Vector3 Direction;

        public LocomotionAction(string name, Vector3 direction) : base(name)
        {
            Direction = direction.normalized;
        }

        public LocomotionAction()
        {
        }

        public LocomotionAction(LocomotionAction src) : base(src)
        {
            Direction = src.Direction;
        }

        public LocomotionAction(LocomotionActionSaveData saveData)
            : base(saveData.ActionBase)
        {
            Direction = saveData.Direction;
        }

        public new LocomotionActionSaveData Save()
        {
            return new LocomotionActionSaveData(
                base.Save(),
                Direction
            );
        }

        public override IActionSaveData SaveAsInterface()
        {
            return Save();
        }

        public override IAction Clone()
        {
            return new LocomotionAction(this);
        }

        private static Vector3 GetLastMovement(State lastState, State nowState)
        {
            if (!(lastState.ContainsKey(State.BasicKeys.Position) && nowState.ContainsKey(State.BasicKeys.Position)))
            {
                throw new ArgumentException("need position");
            }
            if (!(lastState.ContainsKey(State.BasicKeys.Rotation)))
            {
                throw new ArgumentException("need rotation");
            }
            var rotationVector = lastState[State.BasicKeys.Rotation];
            var rotation = new Quaternion(
                (float) rotationVector[0],
                (float) rotationVector[1],
                (float) rotationVector[2],
                (float) rotationVector[3]);

            var lastPosition = lastState.GetAsVector3(State.BasicKeys.Position);
            var currentPosition = nowState.GetAsVector3(State.BasicKeys.Position);

            return Quaternion.Inverse(rotation) * (currentPosition - lastPosition);
        }

        public override float Reward(State lastState, State nowState)
        {
            var movement = GetLastMovement(lastState, nowState);
            return Vector3.Dot(movement, Direction);
        }
    }
}