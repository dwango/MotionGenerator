using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    public class EnergizeSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<EnergizeSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new EnergizeSoul();
        }
    }
}