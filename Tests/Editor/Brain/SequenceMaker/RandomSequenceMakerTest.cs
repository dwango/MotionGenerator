using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace MotionGenerator
{
    public class RandomSequenceMakerTest
    {
        private readonly List<IAction> _dummyActions = new List<IAction> {LocomotionAction.GoStraight("test")};

        [Test]
        public void RandomSequenceTest()
        {
            //Arrange
            const int numControlPoints = 3;
            const int dimention = 3;
            const float maxValue = 3.0f;
            var sequenceMaker = new RandomSequenceMaker(10, maxValue, numControlPoints);

            //Act
            sequenceMaker.Init(
                _dummyActions,
                new List<int> {new ManipulatableMock().GetManipulatableDimention()}
            );
            List<MotionSequence> sequenceDict = sequenceMaker.GenerateSequence(_dummyActions[0]);

            //Assert
            MotionSequence sequence = sequenceDict.First();
            Assert.AreEqual(sequence[0].value.Count, numControlPoints);
            Assert.AreEqual(sequence[0].value.Count, dimention);
            Assert.Less(sequence[0].time, sequence[1].time);
            Assert.Less(sequence[0].value[0], maxValue);
            Assert.Less(sequence[0].value[1], maxValue);
            Assert.Less(sequence[0].value[2], maxValue);
            Assert.Greater(sequence[0].value[0], 0);
            Assert.Greater(sequence[0].value[1], 0);
            Assert.Greater(sequence[0].value[2], 0);
        }

        [Test]
        public void DeepCopyConstructorTest()
        {
            var sequenceMaker = new RandomSequenceMaker(10, 3.0f, 3);
            sequenceMaker.Init(_dummyActions, new List<int> {new ManipulatableMock().GetManipulatableDimention()});
            Assert.IsNotNull(new RandomSequenceMaker(sequenceMaker));
        }
    }
}