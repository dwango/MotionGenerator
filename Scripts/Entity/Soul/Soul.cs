using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public abstract class Soul : ISoul
    {
        public abstract float Reward(State lastState, State nowState);

        public SoulBaseSaveData Save()
        {
            return new SoulBaseSaveData();
        }

        public abstract SoulSaveData SaveAsInterface();

        protected static float Distance(Vector x, Vector y)
        {
            var diff = y - x;
            return (float)Math.Sqrt(diff.DotProduct(diff));
        }
    }
}