using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class ShootUpwardAction : ActionBase
    {
        public ShootUpwardAction(string name) : base(name)
        {
        }

        public ShootUpwardAction() : this("shootUpward")
        {
        }

        public ShootUpwardAction(ShootUpwardAction src) : base(src)
        {
        }

        public ShootUpwardAction(ShootUpwardActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new ShootUpwardActionSaveData Save()
        {
            return new ShootUpwardActionSaveData(
                base.Save()
            );
        }

        public override IActionSaveData SaveAsInterface()
        {
            return Save();
        }

        public override IAction Clone()
        {
            return new ShootUpwardAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            if (!(nowState.ContainsKey(State.BasicKeys.BulletVelocity)))
            {
                throw new ArgumentException("need BulletVelocity");
            }
            var velocity = nowState[State.BasicKeys.BulletVelocity];
            return (float) (velocity.DotProduct(new DenseVector(new double[] {0, 1, 0})) / velocity.Norm(2));
        }
    }
}