using System;

namespace MotionGenerator.Entity.Soul
{
    /// <summary>
    /// 距離センサーを縮めたい系のSoulのbase class
    /// </summary>
    public abstract class DistanceSensorSoulBase : Soul
    {
        protected abstract string Key();

        // Rewardの絶対値が1前後になるようにする調整係数
        protected const float NormalizationCoefficient = 1000;

        /// <summary>
        /// 係数。逃げたいSoulなら1、近づきたいなら-1とか
        /// </summary>
        protected virtual float Coefficient(State nowState)
        {
            return -1f;
        }

        protected static float GetOrganEnergy(State nowState)
        {
            return (float) nowState[State.BasicKeys.OrganEnergy][0];
        }

        public override float Reward(State lastState, State nowState)
        {
            if (nowState.ContainsKey(Key()))
            {
                var currentPosition = nowState[Key()];
                double currentDistance = Math.Sqrt(currentPosition.DotProduct(currentPosition));
                return NormalizationCoefficient * Coefficient(nowState) * (float) currentDistance;
            }
            else
            {
                return 0;
            }
        }
    }

    public abstract class DistanceSensorDifferencialSoulBase : DistanceSensorSoulBase
    {
        public sealed override float Reward(State lastState, State nowState)
        {
            if (lastState.ContainsKey(Key()) && nowState.ContainsKey(Key()))
            {
                var lastVector = lastState[Key()];
                var currentVector = nowState[Key()];
                double lastDistance = Math.Sqrt(lastVector.DotProduct(lastVector));
                double currentDistance = Math.Sqrt(currentVector.DotProduct(currentVector));
                return NormalizationCoefficient * Coefficient(nowState) * (float) (currentDistance - lastDistance);
            }
            else
            {
                return 0;
            }
        }
    }
}