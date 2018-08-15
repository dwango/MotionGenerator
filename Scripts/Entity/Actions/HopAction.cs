using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class HopAction : TurnActionBase
    {
        public HopAction(string name) : base(name)
        {
        }

        public HopAction() : this("hop")
        {
        }

        public HopAction(HopAction src) : base(src)
        {
        }

        public HopAction(HopActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new HopActionSaveData Save()
        {
            return new HopActionSaveData(
                base.Save()
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new HopAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            return (float) nowState[State.BasicKeys.HeightDifferenceInAction][0];
        }
    }
}