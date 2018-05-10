using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public class CrowdDifferencialSoul: DistanceSensorDifferencialSoulBase
    {
        protected override string Key()
        {
            return State.BasicKeys.RelativeTribePosition;
        }
        
        public override ISoulSaveData SaveAsInterface()
        {
            return new CrowdDifferencialSoulSaveData();
        }
    }
}