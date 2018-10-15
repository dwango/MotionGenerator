using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;
using System;

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
                new Dictionary<Guid, int> {{new ManipulatableMock().GetManipulatableId(), 0}},
                new List<int> {new ManipulatableMock().GetManipulatableDimention()}
            );
            var copiedSequenceMaker = new EvolutionarySequenceMaker(0.3f, minimumCandidates: 3);
            copiedSequenceMaker.Init(sequenceMaker,
                new Dictionary<Guid, int> {{new ManipulatableMock().GetManipulatableId(), 0}},
                new List<int> {new ManipulatableMock().GetManipulatableDimention()});

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
                new Dictionary<Guid, int> {{new ManipulatableMock().GetManipulatableId(), 0}},
                new List<int> {(new ManipulatableMock()).GetManipulatableDimention()}
            );

            var src = sm.Save();

            var srcBinary = EditorTestExtensions.SerializeByMsgPack(src);
            var dst = EditorTestExtensions.DeepCloneByMsgPack(src);
            var dstBinary = EditorTestExtensions.SerializeByMsgPack(dst);

            Assert.IsTrue(srcBinary.SequenceEqual(dstBinary));
        }

        [Test]
        public void InheritManipulatorTest()
        {
            InheritableSequenceMakerTestFunctions.InheritManipulatorTest(
                new EvolutionarySequenceMaker(0.3f, 3),
                new EvolutionarySequenceMaker(0.3f, 3));
        }
    }
}