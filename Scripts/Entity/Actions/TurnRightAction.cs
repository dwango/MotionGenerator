using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class TurnRightAction : TurnActionBase
    {
        public TurnRightAction(string name) : base(name)
        {
        }

        public TurnRightAction() : this("turnRight")
        {
        }

        public TurnRightAction(TurnRightAction src) : base(src)
        {
        }

        public TurnRightAction(TurnRightActionSaveData saveData)
            : base(saveData.ActionBase)
        {
        }

        public new TurnRightActionSaveData Save()
        {
            return new TurnRightActionSaveData(
                base.Save()
            );
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            return new TurnRightAction(this);
        }

        public override float Reward(State lastState, State nowState)
        {
            return GetRotatedAngle(lastState, nowState);
        }
    }
}