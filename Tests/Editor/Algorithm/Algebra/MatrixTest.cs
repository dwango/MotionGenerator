using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace MotionGenerator.Tests.Editor.Algorithm.Algebra
{
    public class MatrixTest
    {
        [Test]
        public void AddTest()
        {
            //Arrange
            var a = DenseMatrix.OfArray(new double[,] {{1, 2, 3}, {4, 5, 6}, {7, 9, 8}});

            //Assert
            Assert.AreEqual(a[0, 0], 1);
        }
    }
}