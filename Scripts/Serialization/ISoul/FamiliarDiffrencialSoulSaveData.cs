using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class FamiliarDiffrencialSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<FamiliarDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new FamiliarDiffrencialSoul();
        }
    }

    [MessagePackObject]
    public class ModerateFamiliarDiffrencialSoulSaveData : ISoulSaveData,
        IMotionGeneratorSerializable<ModerateFamiliarDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateFamiliarDiffrencialSoul();
        }
    }
}