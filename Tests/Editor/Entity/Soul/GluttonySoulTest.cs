using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MotionGenerator.Entity.Soul
{
    public class GluttonySoulTest
    {
        [Test]
        public void StateTranlationTest()
        {
            //Arrange
            //Arrange
            var lastState = new State();
            lastState[GluttonySoul.Key] = new DenseVector(new double[] {10});
            var nowState = new State();
            nowState[GluttonySoul.Key] = new DenseVector(new double[] {13});

            //Act
            var reward = (new GluttonySoul()).Reward(lastState, nowState);

            //Assert
            Assert.AreEqual(0.99f, reward);
        }
    }
}