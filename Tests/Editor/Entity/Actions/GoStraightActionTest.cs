using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace MotionGenerator
{
    public class GoStraightActionTest
    {
        [Test]
        public void RewardTest()
        {
            //Arrange
            var action = LocomotionAction.GoStraight("test");
            var lastPosition = new Vector3(1, 2, 3);
            var nowPosition = new Vector3(3, 4, 5);
            var lastState = new State();
            lastState[State.BasicKeys.Position] =
                new DenseVector(new double[] {lastPosition.x, lastPosition.y, lastPosition.z});
            lastState[State.BasicKeys.Rotation] = new DenseVector(new double[] {0, 0, 0, 0});
            var nowState = new State();
            nowState[State.BasicKeys.Position] =
                new DenseVector(new double[] {nowPosition.x, nowPosition.y, nowPosition.z});

            //Act
            var reward = action.Reward(lastState, nowState);

            //Assert
            Assert.AreEqual(reward, 2.0f);
        }
    }
}