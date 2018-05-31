using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class TurnLeftActionSaveData : IActionSaveData, IMotionGeneratorSerializable<TurnLeftActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }

        public TurnLeftActionSaveData()
        {
            
        }

        public TurnLeftActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new TurnLeftAction(this);
        }
    }
}