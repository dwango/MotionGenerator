using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class TurnRightActionSaveData : IActionSaveData, IMotionGeneratorSerializable<TurnRightActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }

        public TurnRightActionSaveData()
        {
            
        }

        public TurnRightActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new TurnRightAction(this);
        }
    }
}