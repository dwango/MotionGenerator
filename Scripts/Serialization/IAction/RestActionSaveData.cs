using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class RestActionSaveData : IActionSaveData, IMotionGeneratorSerializable<RestActionSaveData>
    {
        [Key(0)]
        public ActionBaseSaveData ActionBase { get; set; }

        public RestActionSaveData()
        {
        }

        public RestActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new RestAction(this);
        }
    }
}