using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public abstract class Soul : ISoul
    {
        public abstract float Reward(State lastState, State nowState);

        public virtual ISoulSaveData SaveAsInterface()
        {
            return new SoulBaseSaveData();
        }

        protected static float Distance(Vector x, Vector y)
        {
            var diff = y - x;
            return (float)Math.Sqrt(diff.DotProduct(diff));
        }
    }
}