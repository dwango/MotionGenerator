using MessagePack;

namespace MotionGenerator.Serialization
{
    [Union(0, typeof(ActionBaseSaveData))]
    [Union(1, typeof(LocomotionActionSaveData))]
    [Union(2, typeof(TurnRightActionSaveData))]
    [Union(3, typeof(TurnLeftActionSaveData))]
    [Union(4, typeof(GoForwardCoordinateActionSaveData))]
    [Union(5, typeof(ShootUpwardActionSaveData))]
    [Union(6, typeof(SubDecisionMakerActionSaveData))]
    [Union(7, typeof(StayActionSaveData))]
    [Union(8, typeof(SpinTurnActionSaveData))]
    [Union(9, typeof(RestActionSaveData))]
    [Union(10, typeof(HopActionSaveData))]

    public interface IActionSaveData
    {
        IAction Instantiate();
    }
}
