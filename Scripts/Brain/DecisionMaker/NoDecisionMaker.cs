using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class NoDecisionMaker : DecisionMakerBase
    {
        public NoDecisionMaker()
        {
        }

        public NoDecisionMaker(NoDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
        }

        public override IDecisionMakerSaveData Save()
        {
            return new NoDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save()
            );
        }

        public override void Init(List<IAction> actions)
        {
            base.Init(actions.Take(1).ToList());
        }

        public override IAction DecideAction(State state)
        {
            return Actions[0];
        }
    }
}