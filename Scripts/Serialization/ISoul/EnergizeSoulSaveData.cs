using MotionGenerator.Entity.Soul;
using Serialization;

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