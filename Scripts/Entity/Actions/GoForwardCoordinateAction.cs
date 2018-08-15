using System;
using UnityEngine;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class GoForwardCoordinateAction : ActionBase
    {
        public GoForwardCoordinateAction(string name) : base(name)
        {
        }

        public GoForwardCoordinateAction() : this("forwardCoordinate")
        {
        }

        public GoForwardCoordinateAction(GoForwardCoordinateAction src) : base(src)
        {
        }

        public GoForwardCoordinateAction(GoForwardCoordinateActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new GoForwardCoordinateActionSaveData Save()
        {
            return new GoForwardCoordinateActionSaveData(
                base.Save()
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new GoForwardCoordinateAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            if (!(lastState.ContainsKey(State.BasicKeys.Position) && nowState.ContainsKey(State.BasicKeys.Position)))
            {
                throw new ArgumentException("need position");
            }
            if (!(lastState.ContainsKey(State.BasicKeys.Rotation)))
            {
                throw new ArgumentException("need rotation");
            }
            var rotation = lastState.GetAsQuaternion(State.BasicKeys.Rotation);
            var lastPosition = lastState.GetAsVector3(State.BasicKeys.Position);
            var currentPosition = nowState.GetAsVector3(State.BasicKeys.Position);
            var diffVec = (Quaternion.Inverse(rotation) * (lastPosition) + new Vector3(0.0f, 0.0f, 1.0f) -
                           Quaternion.Inverse(rotation) * currentPosition);
            return 1 / (Vector3.Dot(diffVec, diffVec)); // inverse of distance
        }
    }
}