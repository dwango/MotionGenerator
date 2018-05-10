using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class FamiliarDiffrencialSoulSaveData : ISoulSaveData, IALifeSerializable<FamiliarDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new FamiliarDiffrencialSoul();
        }
    }

    [MessagePackObject]
    public class ModerateFamiliarDiffrencialSoulSaveData : ISoulSaveData,
        IALifeSerializable<ModerateFamiliarDiffrencialSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ModerateFamiliarDiffrencialSoul();
        }
    }
}