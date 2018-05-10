using NUnit.Framework;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Serialization;

namespace MotionGenerator
{
    public class FollowHighestDensityDecisionMakerTest
    {
        private readonly List<IAction> _actions = LocomotionAction.EightDirections();

        private FollowHighestDensityDecisionMaker createDummy(bool isNegative = false)
        {
            var decisionMaker = new FollowHighestDensityDecisionMaker(State.BasicKeys.TotalFoodEnergyEachDirection, isNegative);
            decisionMaker.Init(_actions);
            return decisionMaker;
        }

        private State TempState()
        {
            var state = new State();
            state[State.BasicKeys.TotalFoodEnergyEachDirection] = new DenseVector(8);
            return state;
        }

        [Test]
        public void 前にエネルギーがあるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][0] = 1f;
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoStraight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 後にエネルギーがあるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][4] = 1f;
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoBack().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 右にエネルギーがあるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            for (var y = -1; y <= 1; y++) {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][2] = 1f;
                var action = decisionMaker.DecideAction (tmpState);
                Assert.AreEqual (
                    LocomotionAction.GoRight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 右前にエネルギーがあるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            for (var y = -1; y <= 1; y++) {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][1] = 1f;
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoForwardRight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 左にエネルギーがあるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            for (var y = -1; y <= 1; y++) {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][6] = 1f;
                var action = decisionMaker.DecideAction (tmpState);
                Assert.AreEqual (
                    LocomotionAction.GoLeft().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 反対側をちゃんと選ぶ()
        {
            var decisionMaker = createDummy(isNegative: true);
            var tmpState = TempState();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][0] = 1f;
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoBack().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void コピーコンストラクタでつくられた親子は同じDecisionをする()
        {
            var decisionMaker = createDummy();
            var tmpState = TempState();
            tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][6] = 1f;
            var decisionMakerClone = new FollowHighestDensityDecisionMaker(State.BasicKeys.TotalFoodEnergyEachDirection);
            decisionMakerClone.Init(decisionMaker);

            var action1 = decisionMaker.DecideAction(tmpState);
            var action2 = decisionMakerClone.DecideAction(tmpState);
            Assert.AreEqual(
                action1.Name,
                action2.Name
            );
        }

        [Test]
        public void デシリアライズ後Restoreされたものは同じDecisionをする()
        {
            var decisionMaker = createDummy();

            var saveData = decisionMaker.Save();
            var clonedSaveData = EditorTestExtensions.DeepCloneByMsgPack(saveData);

            var decisionMakerClone = clonedSaveData.Instantiate();
            decisionMakerClone.Restore(_actions);

            var tmpState = TempState();
            tmpState[State.BasicKeys.TotalFoodEnergyEachDirection][6] = 1f;
            var action1 = decisionMaker.DecideAction(tmpState);
            var action2 = decisionMakerClone.DecideAction(tmpState);

            Assert.AreEqual(
                action1.Name,
                action2.Name
            );
        }
    }
}