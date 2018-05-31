using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class StayActionSaveData : IActionSaveData, IMotionGeneratorSerializable<StayActionSaveData>
    {
        [Key(0)]
        public ActionBaseSaveData ActionBase { get; set; }

        public StayActionSaveData()
        {
        }

        public StayActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new StayAction(this);
        }
    }
}