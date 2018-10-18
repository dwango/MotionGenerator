﻿using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;

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
            var sequenceMaker = new RandomSequenceMaker(10, maxValue, numControlPoints,
                new Dictionary<Guid, int> {{Guid.NewGuid(), (new ManipulatableMock()).GetManipulatableDimention()}});

            //Act
            sequenceMaker.Init(
                _dummyActions,
                new Dictionary<Guid, int> {{Guid.NewGuid(), new ManipulatableMock().GetManipulatableDimention()}}
            );
            var sequenceDict = sequenceMaker.GenerateSequence(_dummyActions[0]);

            //Assert
            var sequence = sequenceDict.Values.First();
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
            var sequenceMaker = new RandomSequenceMaker(10, 3.0f, 3, 
                new Dictionary<Guid, int> {{Guid.NewGuid(), new ManipulatableMock().GetManipulatableDimention()}});
            sequenceMaker.Init(_dummyActions,
                new Dictionary<Guid, int> {{Guid.NewGuid(), new ManipulatableMock().GetManipulatableDimention()}});
            Assert.IsNotNull(new RandomSequenceMaker(sequenceMaker));
        }
    }
}