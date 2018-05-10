using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    public class EnergizeSoulSaveData : ISoulSaveData, IALifeSerializable<EnergizeSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new EnergizeSoul();
        }
    }
}