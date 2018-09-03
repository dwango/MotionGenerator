using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public class CrowdDifferencialSoul: DistanceSensorDifferencialSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeTribePosition;
        }
        
        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
}