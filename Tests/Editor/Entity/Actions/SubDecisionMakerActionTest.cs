using NUnit.Framework;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class SubDecisionMakerActionTest
    {
        [Test]
        public void SerializeDeserializeでDecisonMakerの状態が復元する()
        {
            var action = new SubDecisionMakerAction(new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition));
            var cloneAction = action.Save().Instantiate() as SubDecisionMakerAction;
//
            Assert.IsTrue(action.DecisionMaker is FollowPointDecisionMaker);
            Assert.IsTrue(cloneAction.DecisionMaker is FollowPointDecisionMaker);
        }
    }
}