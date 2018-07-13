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

    internal class LocomotionSequenceMakerMock : LocomotionSequenceMaker
    {
        public LocomotionSequenceMakerMock(float epsilon, int minimumCandidates, float timeScale,
            ISequenceMaker fallbackSequenceMaker, bool enableTurn = false) : base(epsilon, minimumCandidates, timeScale,
            fallbackSequenceMaker, enableTurn)
        {
        }

        public void SetCandidates(List<Candidate3D> candidates)
        {
            Candidates = candidates;
        }

        public List<Candidate3D> GetCandidates()
        {
            return Candidates;
        }
    }

    public class LocomotionSequenceMakerTest
    {
        private State dummyState = new State(new Dictionary<string, Vector>()
        {
            {State.BasicKeys.Position, Vector.Build.Dense(new double[] {1, 1, 1}) as Vector},
            {State.BasicKeys.Rotation, Vector.Build.Dense(new double[] {1, 1, 1, 1}) as Vector},
            {State.BasicKeys.Time, Vector.Build.Dense(new double[] {1}) as Vector},
        });

        [Test]
        public void 方向以外のActionがきた時にもFallbackしてSequenceが作れる()
        {
            var actions = new List<IAction> {new TurnLeftAction("test")};
            var fallbackSequencMaker = new EvolutionarySequenceMakerSpy(0.3f, 30);
            var sequenceMaker = new LocomotionSequenceMaker(epsilon: 0.3f, minimumCandidates: 10, timeScale: 3,
                fallbackSequenceMaker: fallbackSequencMaker);
            sequenceMaker.Init(actions, new List<int> {new ManipulatableMock().GetManipulatableDimention()});
            sequenceMaker.GenerateSequence(actions[0]);
            sequenceMaker.Feedback(10, new State(), new State());

            Assert.IsTrue(fallbackSequencMaker.gotFeedback);
        }

        [Test]
        public void MinimumDurationFrame以下のcandidatesについてはそれ以上短くならない()
        {
            var actions = new List<IAction> {LocomotionAction.GoStraight()};
            // epsilonを大きくして必ず変異
            var sequenceMaker = new LocomotionSequenceMakerMock(epsilon: 100f, minimumCandidates: 10, timeScale: 3,
                fallbackSequenceMaker: new EvolutionarySequenceMakerSpy(0.3f, 30));
            var manipulable = new ManipulatableMock();
            sequenceMaker.Init(actions, new List<int> {manipulable.GetManipulatableDimention()});


            const int initialDurationFrame = 10;
            var dummyCandidate = new Candidate3D(new List<MotionSequence>()
            {
                new MotionSequence(new List<MotionTarget>()
                {
                    new MotionTarget(
                        initialDurationFrame,
                        Enumerable.Range(0, manipulable.GetManipulatableDimention()).Select(_ => 1f).ToList()
                    ),
                    new MotionTarget(
                        initialDurationFrame,
                        Enumerable.Range(0, manipulable.GetManipulatableDimention()).Select(_ => 1f).ToList()
                    ),
                    new MotionTarget(
                        initialDurationFrame,
                        Enumerable.Range(0, manipulable.GetManipulatableDimention()).Select(_ => 1f).ToList()
                    )
                })
            });
            sequenceMaker.SetCandidates(new List<Candidate3D>() {dummyCandidate});

            // Do 100 action
            for (int i = 0; i < 100; i++)
            {
                sequenceMaker.GenerateSequence(actions[0]);
                sequenceMaker.Feedback(10, dummyState, dummyState);
            }

            // Assertion
            var resultCandidates = sequenceMaker.GetCandidates();
            Assert.AreEqual(101, resultCandidates.Count);
            for (int i = 1; i < 101; i++)
            {
                var duration = resultCandidates[i].Value.Select(motionSequence => motionSequence.GetDuration()).Max();
                Assert.Greater(duration, initialDurationFrame);
            }
        }
    }
}