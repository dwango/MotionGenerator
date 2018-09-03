using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    /// <summary>
    /// フードに近ければ嬉しい
    /// </summary>
    public class SnufflingSoul : DistanceSensorSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    /// <summary>
    /// 草食動物に近ければ嬉しい
    /// </summary>
    public class SnufflingFleshSoul : DistanceSensorSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeFleshFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    /// <summary>
    /// フードに近づくと嬉しい
    /// 実はこちらを使ったほうが圧倒的に学習が早い
    /// (状態価値を与えてることに近くなるからかと)
    /// </summary>
    public class SnufflingDifferencialSoul : DistanceSensorDifferencialSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    /// <summary>
    /// 草食動物に近づくと嬉しい
    /// </summary>
    public class SnufflingFleshDifferencialSoul : DistanceSensorDifferencialSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeFleshFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
    
    /// <summary>
    /// フードに近づくと嬉しい【現在エネルギー保持率で補正する】
    /// </summary>
    public class ModerateSnufflingDifferencialSoul : DistanceSensorDifferencialSoulBase
    {
        private const float ShortEnergyRatio = 0.5f; // 空腹をどれだけ重くみるか
        private const float NegativeShortEnergyRatio = 1f - ShortEnergyRatio;
        
        protected override float Coefficient(State nowState)
        {
            // 空腹度の二乗でうれしがる
            var shortEnergy = 1f - GetOrganEnergy(nowState);
            return -NegativeShortEnergyRatio - shortEnergy * shortEnergy * ShortEnergyRatio;
        }

        protected override string Key()
        {
            return State.BasicKeys.RelativeFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    /// <summary>
    /// 草食動物に近づくと嬉しい【現在エネルギー保持率で補正する】
    /// </summary>
    public class ModerateSnufflingFleshDifferencialSoul : DistanceSensorDifferencialSoulBase
    {
        private const float ShortEnergyRatio = 0.5f; // 空腹をどれだけ重くみるか
        private const float NegativeShortEnergyRatio = 1f - ShortEnergyRatio;
        
        protected override float Coefficient(State nowState)
        {
            // 空腹度の二乗でうれしがる
            var shortEnergy = 1f - GetOrganEnergy(nowState);
            return -NegativeShortEnergyRatio - shortEnergy * shortEnergy * ShortEnergyRatio;
        }

        protected override string Key()
        {
            return State.BasicKeys.RelativeFleshFoodPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
}