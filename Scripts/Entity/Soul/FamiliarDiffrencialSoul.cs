using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public class FamiliarDiffrencialSoul : DistanceSensorDifferencialSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeStrangerPosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    public class ModerateFamiliarDiffrencialSoul : DistanceSensorDifferencialSoulBase
    {
        private const float ShortEnergyRatio = 0.5f; // 空腹をどれだけ重くみるか
        private const float NegativeShortEnergyRatio = 1f - ShortEnergyRatio;

        protected override float Coefficient(State nowState)
        {
            var shortEnergy = 1f - GetOrganEnergy(nowState);
            return -NegativeShortEnergyRatio - shortEnergy * ShortEnergyRatio;
        }

        protected override string Key()
        {
            return State.BasicKeys.RelativeCreaturePosition;
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
}