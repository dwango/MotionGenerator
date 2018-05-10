using MessagePack;
using MotionGenerator.Entity.Soul;

namespace MotionGenerator.Serialization
{
    [Union(0, typeof(SoulBaseSaveData))]
    [Union(1, typeof(GluttonySoulSaveData))]
    [Union(2, typeof(SnufflingSoulSaveData))]
    [Union(3, typeof(SnufflingDifferencialSoulSaveData))]
    [Union(4, typeof(SnufflingFleshSoulSaveData))]
    [Union(5, typeof(SnufflingFleshDifferencialSoulSaveData))]
    [Union(6, typeof(ShoutingLoveSoulSaveData))]
    [Union(7, typeof(ShoutingLoveDifferencialSoulSaveData))]
    [Union(8, typeof(TerritorySoulSaveData))]
    [Union(9, typeof(NostalgiaSoulSaveData))]
    [Union(10, typeof(CrowdDifferencialSoulSaveData))]
    [Union(11, typeof(CowardSoulSaveData))]
    [Union(12, typeof(CowardDiffrencialSoulSaveData))]
    [Union(13, typeof(BumpingDifferenceialSoulSaveData))]
    [Union(14, typeof(LazySoulSaveData))]
    [Union(15, typeof(FamiliarDiffrencialSoulSaveData))]
    [Union(16, typeof(ModerateSnufflingDifferencialSoulSaveData))]
    [Union(17, typeof(ModerateSnufflingFleshDifferencialSoulSaveData))]
    [Union(18, typeof(ModerateFamiliarDiffrencialSoulSaveData))]
    [Union(19, typeof(ModerateBumpingDifferenceialSoulSaveData))]
    [Union(20, typeof(AvoidObjectSoulSaveData))]
    public interface ISoulSaveData
    {
        ISoul Instantiate();
    }
}