using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace MotionGenerator.Entity.Soul
{
    public class SnufflingSoulTest
    {
        [Test]
        public void 草食動物との距離に応じた報酬が得られる()
        {
            //Arrange
            var lastState = new State();
            var nowState = new State();
            nowState[State.BasicKeys.RelativeFleshFoodPosition] = new DenseVector(new double[] {1, 2, 3});

            //Act
            var reward = (new SnufflingFleshSoul()).Reward(lastState, nowState);

            //Assert
            Assert.AreEqual(-3.7416573867739413f * 1000, reward);
        }

        [Test]
        public void 草食動物との距離の差分に応じた報酬が得られる()
        {
            //Arrange
            var lastState = new State();
            var nowState = new State();
            lastState[State.BasicKeys.RelativeFleshFoodPosition] = new DenseVector(new double[] {3, 3, 3});
            nowState[State.BasicKeys.RelativeFleshFoodPosition] = new DenseVector(new double[] {1, 2, 3});

            //Act
            var reward = (new SnufflingFleshDifferencialSoul()).Reward(lastState, nowState);

            //Assert
            Assert.AreEqual(1.4544950359326907f * 1000, reward, delta: 0.01f);
        }
    }
}