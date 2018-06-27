using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class HopActionSaveData : IActionSaveData, IMotionGeneratorSerializable<HopActionSaveData>
    {
        [Key(0)]
        public ActionBaseSaveData ActionBase { get; set; }

        public HopActionSaveData()
        {
        }

        public HopActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new HopAction(this);
        }
    }
}