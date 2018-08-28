using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class RestAction : TurnActionBase
    {
        public RestAction(string name) : base(name)
        {
        }

        public RestAction() : this("rest")
        {
        }

        public RestAction(RestAction src) : base(src)
        {
        }

        public RestAction(RestActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new RestActionSaveData Save()
        {
            return new RestActionSaveData(
                base.Save()
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new RestAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            var actionTime = GetActionTime(lastState, nowState);
            var manipulatorEnergyConsumption = GetAverageManipulatorEnergyConsumption(nowState);

            return -manipulatorEnergyConsumption / actionTime;
        }
    }
}