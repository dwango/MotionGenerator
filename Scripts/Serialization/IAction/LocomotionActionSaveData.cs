using MessagePack;
using Serialization;
using UnityEngine;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class LocomotionActionSaveData : IActionSaveData, IALifeSerializable<LocomotionActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }
        [Key(1)] public Vector3 Direction { get; set; }

        public LocomotionActionSaveData()
        {
            
        }

        public LocomotionActionSaveData(ActionBaseSaveData actionBase, Vector3 direction)
        {
            ActionBase = actionBase;
            Direction = direction;
        }

        public LocomotionAction InstantiateLocomotionAction()
        {
            return new LocomotionAction(this);
        }

        public IAction Instantiate()
        {
            return new LocomotionAction(this);
        }
    }
}