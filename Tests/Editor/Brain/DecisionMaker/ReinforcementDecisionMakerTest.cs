using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Algorithm.Reinforcement;
using MotionGenerator.Tests;
using NUnit.Framework;
using Serialization;

namespace MotionGenerator
{
    public class ReinforcementDecisionMakerTest
    {
        private List<State> _dummyStates = new List<State>()
        {
            new State(new Dictionary<string, Vector>()
            {
                {
                    State.BasicKeys.RelativeFoodPosition, new DenseVector(new double[] {0.5f, 0, 0})
                }
            }),
            new State(new Dictionary<string, Vector>()
            {
                {
                    State.BasicKeys.RelativeFoodPosition, new DenseVector(new double[] {0, 0.5f, 0})
                }
            }),
            new State(new Dictionary<string, Vector>()
            {
                {
                    State.BasicKeys.RelativeFoodPosition, new DenseVector(new double[] {0.9f, 0f, 0.9f})
                }
            }),
            new State(new Dictionary<string, Vector>()
            {
                {
                    State.BasicKeys.RelativeFoodPosition, new DenseVector(new double[] {0, 0, 0.5f})
                }
            })
        };


        [Test]
        public void 遺伝によるコピーで作られたDecisionMakerは完全に同じ動きをする()
        {
            // Init parent
            var parentDecisionMaker = new ReinforcementDecisionMaker();
            var actions = new List<IAction>();
            for (int i = 0; i < 100; i++)
            {
                actions.Add(LocomotionAction.GoStraight(i.ToString()));
            }
            parentDecisionMaker.Init(actions);
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);


            // Init child
            var childDecisionMaker = new ReinforcementDecisionMaker();
            childDecisionMaker.Init(parentDecisionMaker);

            // Assertion
            foreach (var state in _dummyStates)
            {
                Assert.AreEqual(
                    parentDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true)
                        .Name, // choose from 100 choice
                    childDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true).Name
                );
            }
        }


        [Test]
        public void SubDecisionMakerを使って意思決定ができ遺伝もできる()
        {
            // Init parent
            var parentDecisionMaker = new ReinforcementDecisionMaker();
            var actions = new List<IAction>();
            actions.AddRange(LocomotionAction.EightDirections());
            for (int i = 0; i < 100; i++)
            {
                actions.Add(
                    new SubDecisionMakerAction(new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition)));
            }
            parentDecisionMaker.Init(actions);
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);

            // Init child
            var childDecisionMaker = new ReinforcementDecisionMaker();
            childDecisionMaker.Init(parentDecisionMaker);

            // Assertion
            foreach (var state in _dummyStates)
            {
                var parentAction = parentDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true);
                var childAction = childDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true);
                Assert.AreEqual(
                    parentAction.Name,
                    childAction.Name
                );
                
                // SubDecisionMakerActionは外に出ない
                Assert.IsInstanceOf<LocomotionAction>(parentAction);
                Assert.IsInstanceOf<LocomotionAction>(childAction);
            }
        }

        [Test]
        public void デシリアライズ後Restoreされたものは同じDecisionをする()
        {
            // Init parent
            var parentDecisionMaker = new ReinforcementDecisionMaker();
            var actions = new List<IAction>();
            for (int i = 0; i < 100; i++)
            {
                actions.Add(LocomotionAction.GoStraight(i.ToString()));
            }
            parentDecisionMaker.Init(actions);
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);

            var saveDataClone = EditorTestExtensions.DeepCloneByMsgPack(parentDecisionMaker.Save());
            var decisionMakerClone = saveDataClone.Instantiate() as ReinforcementDecisionMaker;

            // Assertion
            foreach (var state in _dummyStates)
            {
                Assert.AreEqual(
                    parentDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true)
                        .Name, // choose from 100 choice
                    decisionMakerClone.DecideAction(state, forceRandom: false, forceMax: true).Name
                );
            }
        }

        [Test]
        public void デシリアライズ後RestoreされたものはSubDMも含めて同じDecisionをする()
        {
            // Init parent
            var parentDecisionMaker = new ReinforcementDecisionMaker();
            var actions = new List<IAction>();
            actions.Add(new SubDecisionMakerAction(new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition)));
            actions.AddRange(LocomotionAction.EightDirections());
            parentDecisionMaker.Init(actions);
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);

            var saveDataClone = EditorTestExtensions.DeepCloneByMsgPack(parentDecisionMaker.Save());
            var decisionMakerClone = saveDataClone.Instantiate() as ReinforcementDecisionMaker;
            decisionMakerClone.Restore(actions);

            // Random
            foreach (var state in _dummyStates)
            {
                Assert.AreEqual(
                    parentDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true)
                        .Name, 
                    decisionMakerClone.DecideAction(state, forceRandom: false, forceMax: true).Name
                );
            }
            // Force SubDM
            foreach (var state in _dummyStates)
            {
                Assert.AreEqual(
                    parentDecisionMaker.DecideAction(state, forceRandom: false, forceMax: true, forceAction:0)
                        .Name, // choose from 100 choice
                    decisionMakerClone.DecideAction(state, forceRandom: false, forceMax: true, forceAction: 0).Name
                );
            }
        }

        [Test]
        public void デシリアライズ後Restoreされたものは経験を引き継ぐ()
        {
            // Init parent
            var parentDecisionMaker = new ReinforcementDecisionMaker();
            var actions = new List<IAction>();
            for (int i = 0; i < 100; i++)
            {
                actions.Add(LocomotionAction.GoStraight(i.ToString()));
            }
            parentDecisionMaker.Init(actions);
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);
            parentDecisionMaker.Feedback(new List<float>(){3f});
            parentDecisionMaker.DecideAction(_dummyStates[0], forceRandom: false, forceMax: true);

            var saveDataClone = EditorTestExtensions.DeepCloneByMsgPack(parentDecisionMaker.Save());
            var decisionMakerClone = saveDataClone.Instantiate() as ReinforcementDecisionMaker;


            var originalTrainer = TestHelper.GetFieldValue(parentDecisionMaker, "_trainer") as TemporalDifferenceQTrainer;
            var cloneTrainer = TestHelper.GetFieldValue(decisionMakerClone, "_trainer") as TemporalDifferenceQTrainer;
            
            // Assertion
            Assert.AreEqual(originalTrainer.GetHistorySaveData().Count, 1);
            Assert.AreEqual(cloneTrainer.GetHistorySaveData().Count, 1);
            Assert.AreEqual(originalTrainer.GetHistorySaveData()[0].Instantiate().State, cloneTrainer.GetHistorySaveData()[0].Instantiate().State);
        }

    }
}