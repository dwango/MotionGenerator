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

        public override ISoulSaveData SaveAsInterface()
        {
            return new CowardSoulSaveData();
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

        public override ISoulSaveData SaveAsInterface()
        {
            return new CowardDiffrencialSoulSaveData();
        }
    }
}