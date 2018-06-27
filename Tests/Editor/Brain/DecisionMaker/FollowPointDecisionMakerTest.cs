using NUnit.Framework;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class FollowPointDecisionMakerTest
    {
        private readonly List<IAction> _actions = LocomotionAction.EightDirections();

        private FollowPointDecisionMaker createDummy(bool isNegative = false)
        {
            var decisionMaker = new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition, isNegative: isNegative);
            decisionMaker.Init(_actions);
            return decisionMaker;
        }

        [Test]
        public void 前に餌があるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = new State();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {0.5f, y * 100, 2});
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoStraight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 後に餌があるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = new State();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {0.5f, y * 100, -2});
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoBack().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 右に餌があるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = new State();
            for (var y = -1; y <= 1; y++) {
                tmpState [State.BasicKeys.RelativeFoodPosition] = new DenseVector (new double[] { 2, y * 100, 0.5f });
                var action = decisionMaker.DecideAction (tmpState);
                Assert.AreEqual (
                    LocomotionAction.GoRight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 右前に餌があるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = new State();
            for (var y = -1; y <= 1; y++) {
                tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {1, y * 100, 1});
                var action = decisionMaker.DecideAction(tmpState);
                Assert.AreEqual(
                    LocomotionAction.GoForwardRight().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 左に餌があるときはちゃんとそちらに向かう()
        {
            var decisionMaker = createDummy();
            var tmpState = new State();
            for (var y = -1; y <= 1; y++) {
                tmpState [State.BasicKeys.RelativeFoodPosition] = new DenseVector (new double[] { -2, y * 100, 0.5f });
                var action = decisionMaker.DecideAction (tmpState);
                Assert.AreEqual (
                    LocomotionAction.GoLeft().Name,
                    action.Name
                );
            }
        }

        [Test]
        public void 反対側にちゃんと向かう()
        {
            var decisionMaker = createDummy(isNegative: true);
            var tmpState = new State();
            for (var y = -1; y<=1; y++)
            {
                tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {0.5f, y * 100, 2});
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
            var tmpState = new State();
            tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {1, 0, 2});
            var decisionMakerClone = new FollowPointDecisionMaker(State.BasicKeys.RelativeFoodPosition);
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

            var tmpState = new State();
            tmpState[State.BasicKeys.RelativeFoodPosition] = new DenseVector(new double[] {1, 0, 2});
            var action1 = decisionMaker.DecideAction(tmpState);
            var action2 = decisionMakerClone.DecideAction(tmpState);

            Assert.AreEqual(
                action1.Name,
                action2.Name
            );
        }
    }
}