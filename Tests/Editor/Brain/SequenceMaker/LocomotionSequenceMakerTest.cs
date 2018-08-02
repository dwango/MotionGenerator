using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace MotionGenerator
{
    internal class EvolutionarySequenceMakerSpy : EvolutionarySequenceMaker
    {
        public bool gotFeedback = false;

        public EvolutionarySequenceMakerSpy(float epsilon, int minimumCandidates) : base(epsilon, minimumCandidates)
        {
        }

        public override void Feedback(float reward, State lastState, State currentState)
        {
            gotFeedback = true;
            base.Feedback(reward, lastState, currentState);
        }
    }

    public class LocomotionSequenceMakerTest
    {
        [Test]
        public void 方向以外のActionがきた時にもFallbackしてSequenceが作れる()
        {
            var actions = new List<IAction> {new TurnLeftAction("test")};
            var fallbackSequencMaker = new EvolutionarySequenceMakerSpy(0.3f, 30);
            var sequenceMaker = new LocomotionSequenceMaker(epsilon: 0.3f, minimumCandidates: 10, timeScale: 3,
                fallbackSequenceMaker: fallbackSequencMaker);
            sequenceMaker.Init(actions, new List<int> {new ManipulatableMock().GetManipulatableDimention()});
            sequenceMaker.GenerateSequence(actions[0], new State());
            sequenceMaker.Feedback(10, new State(), new State());

            Assert.IsTrue(fallbackSequencMaker.gotFeedback);
        }
    }
}