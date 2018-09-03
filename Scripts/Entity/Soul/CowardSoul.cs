using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public class CowardSoul : DistanceSensorSoulBase
    {
        protected override float Coefficient(State nowState)
        {
            return 1f;
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

    public class CowardDiffrencialSoul : DistanceSensorDifferencialSoulBase
    {
        protected override float Coefficient(State nowState)
        {
            return 1f;
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