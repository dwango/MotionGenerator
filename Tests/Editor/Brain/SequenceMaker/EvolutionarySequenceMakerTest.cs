using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class EvolutionarySequenceMakerTest
    {
        List<IAction> dummyActions = new List<IAction> {LocomotionAction.GoStraight("test")};
        List<IAction> dummyActions2 = new List<IAction> {LocomotionAction.GoStraight("test")};

        [Test]
        public void DeepCopyTest()
        {
            var sequenceMaker = new EvolutionarySequenceMaker(0.3f, minimumCandidates: 3);
            sequenceMaker.Init(
                dummyActions,
                new List<int> {(new ManipulatableMock()).GetManipulatableDimention()}
            );
            var copiedSequenceMaker = new EvolutionarySequenceMaker(0.3f, minimumCandidates: 3);
            copiedSequenceMaker.Init(sequenceMaker);

            sequenceMaker.GenerateSequence(dummyActions[0]);
            copiedSequenceMaker.GenerateSequence(dummyActions[0]);
            copiedSequenceMaker.GenerateSequence(dummyActions2[0]);
        }

        [Test]
        public void SerializationTest()
        {
            var sm = new EvolutionarySequenceMaker(0.3f, 3);
            sm.Init(
                dummyActions,
                new List<int> {(new ManipulatableMock()).GetManipulatableDimention()}
            );

            var src = sm.Save();

            var srcBinary = EditorTestExtensions.SerializeByMsgPack(src);
            var dst = EditorTestExtensions.DeepCloneByMsgPack(src);
            var dstBinary = EditorTestExtensions.SerializeByMsgPack(dst);

            Assert.IsTrue(srcBinary.SequenceEqual(dstBinary));
        }

        [Test]
        public void AlterManipulatablesTest()
        {
            var sm = new EvolutionarySequenceMaker(0.3f, 3);
            sm.Init(
                dummyActions,
                new List<int> {(new ManipulatableMock()).GetManipulatableDimention()}
            );

            var beforeValue = sm.GenerateSequence(dummyActions[0])[0].Sequence[0].value.Count;

            sm.AlterManipulatables(
                new List<int> {(new ManipulatableMock4Dimention()).GetManipulatableDimention()}
            );

            var afterValue = sm.GenerateSequence(dummyActions[0])[0].Sequence[0].value.Count;
            Assert.IsTrue(
                beforeValue != afterValue);
        }

        [Test]
        public void NeedToAlterManipulatablesTest()
        {
            var oldManipulatable = new List<int> {new ManipulatableMock().GetManipulatableDimention()};
            var newManipulatable = new List<int> {new ManipulatableMock4Dimention().GetManipulatableDimention()};

            var sm = new EvolutionarySequenceMaker(0.3f, 3);
            sm.Init(
                dummyActions,
                oldManipulatable
            );

            Assert.IsFalse(sm.NeedToAlterManipulatables(oldManipulatable));
            Assert.IsTrue(sm.NeedToAlterManipulatables(newManipulatable));

            sm.AlterManipulatables(newManipulatable);

            Assert.IsFalse(sm.NeedToAlterManipulatables(newManipulatable));
            Assert.IsTrue(sm.NeedToAlterManipulatables(oldManipulatable));
        }
    }
}