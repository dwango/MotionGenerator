using System;
using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    /// <summary>
    /// 世界の中心にいると嬉しい(デバッグ用)
    /// </summary>
    public class ShoutingLoveSoul : Soul
    {
        protected const string Key = State.BasicKeys.Position;

        public override float Reward(State lastState, State nowState)
        {
            if (nowState.ContainsKey(Key))
            {
                var currentPosition = nowState[Key];

                double currentDistance = Math.Sqrt(currentPosition.DotProduct(currentPosition));
                return (float) -currentDistance;
            }
            else
            {
                return 0;
            }
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    /// <summary>
    /// 世界の中心に近づくと嬉しい(デバッグ用)
    /// 実はこちらを使ったほうが圧倒的に学習が早い
    /// (状態価値を与えてることに近くなるからかと)
    /// </summary>
    public class ShoutingLoveDifferencialSoul : ShoutingLoveSoul
    {
        public override float Reward(State lastState, State nowState)
        {
            if (lastState.ContainsKey(Key) && nowState.ContainsKey(Key))
            {
                double lastDistance = Math.Sqrt(lastState[State.BasicKeys.Position]
                    .DotProduct(lastState[State.BasicKeys.Position]));
                double currentDistance = Math.Sqrt(nowState[State.BasicKeys.Position]
                    .DotProduct(nowState[State.BasicKeys.Position]));
                return (float) (lastDistance - currentDistance);
            }
            else
            {
                return 0;
            }
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
}