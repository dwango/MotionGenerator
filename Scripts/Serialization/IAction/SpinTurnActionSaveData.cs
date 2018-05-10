using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SpinTurnActionSaveData : IActionSaveData, IALifeSerializable<SpinTurnActionSaveData>
    {
        [Key(0)]
        public ActionBaseSaveData ActionBase { get; set; }
        [Key(1)]
        public float TargetAngle { get; set; }
        [Key(2)]
        public float AcceptableMovement { get; set; }

        public SpinTurnActionSaveData()
        {
        }

        public SpinTurnActionSaveData(ActionBaseSaveData actionBase, float targetAngle, float acceptableMovement)
        {
            ActionBase = actionBase;
            TargetAngle = targetAngle;
            AcceptableMovement = acceptableMovement;
        }

        public IAction Instantiate()
        {
            return new SpinTurnAction(this);
        }
    }
}