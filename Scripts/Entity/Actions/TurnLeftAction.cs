using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class TurnLeftAction : TurnActionBase
    {
        public TurnLeftAction(string name) : base(name)
        {
        }

        public TurnLeftAction() : this("turnLeft")
        {
        }

        public TurnLeftAction(TurnLeftAction src) : base(src)
        {
        }

        public TurnLeftAction(TurnLeftActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new TurnLeftActionSaveData Save()
        {
            return new TurnLeftActionSaveData(
                base.Save()
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new TurnLeftAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            return -GetRotatedAngle(lastState, nowState);
        }
    }
}