using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ActionBaseSaveData : IActionSaveData, IALifeSerializable<ActionBaseSaveData>
    {
        [Key(0)] public string Name { get; set; }

        public ActionBaseSaveData()
        {
            
        }

        public ActionBaseSaveData(string name)
        {
            Name = name;
        }

        public IAction Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}