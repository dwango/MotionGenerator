using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ShootUpwardActionSaveData : IActionSaveData, IMotionGeneratorSerializable<ShootUpwardActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }

        public ShootUpwardActionSaveData()
        {
            
        }

        public ShootUpwardActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new ShootUpwardAction(this);
        }
    }
}