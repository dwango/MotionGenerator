using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class SubDecisionMakerAction : ActionBase
    {
        public readonly IDecisionMaker DecisionMaker;

        public SubDecisionMakerAction(IDecisionMaker decisionMaker) : base(decisionMaker.GetHashCode().ToString())
        {
            DecisionMaker = decisionMaker;
        }

        public SubDecisionMakerAction(SubDecisionMakerActionSaveData saveData) : base(saveData.ActionBase)
        {
            DecisionMaker = saveData.DecisionMakerSaveData.Instantiate();
        }

        public new SubDecisionMakerActionSaveData Save()
        {
            return new SubDecisionMakerActionSaveData(base.Save(), DecisionMaker.Save());
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override IAction Clone()
        {
            throw new System.NotImplementedException();
        }

        public override float Reward(State lastState, State nowState)
        {
            throw new System.NotImplementedException();
        }
    }
}