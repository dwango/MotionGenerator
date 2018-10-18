using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;

namespace MotionGenerator
{
    public class SimpleBanditSequenceMakerTest
    {
        List<IAction> dummyActions = new List<IAction> {LocomotionAction.GoStraight("test")};

        [Test]
        public void 単なる結合テスト()
        {
            var sequenceMaker = new SimpleBanditSequenceMaker(0.3f, minimumCandidates: 3);
            sequenceMaker.Init(
                dummyActions,
                new Dictionary<Guid, int> {{Guid.NewGuid(), new ManipulatableMock().GetManipulatableDimention()}}
            );
            var sequence = sequenceMaker.GenerateSequence(dummyActions[0]);
            Assert.AreEqual(sequence.Count, 1);
            Assert.AreEqual(sequence.First().Value[0].value.Count, (new ManipulatableMock()).GetManipulatableDimention());
        }
    }
}