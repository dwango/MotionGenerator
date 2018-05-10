using MathNet.Numerics.Distributions;
using MotionGenerator.Algorithm.Reinforcement;
using NUnit.Framework;
using Serialization;

namespace MotionGenerator.Tests.Editor.Algorithm.Reinforcement
{
    public class TemporalDifferenceQTrainerTest
    {
        [Test]
        public void CanSerializeAndDeserializeParameters()
        {
            var parameter = new TemporalDifferenceQTrainerParameter(
                new float[] {1, 2, 3}, action: 3,
                state: MathNet.Numerics.LinearAlgebra.Single.DenseMatrix.CreateRandom(3, 3, new Normal()),
                nextState: MathNet.Numerics.LinearAlgebra.Single.DenseMatrix.CreateRandom(3, 3, new Normal())
            );

            var saveData = parameter.Save();
            var copiedSaveData = ALifeSerialization.DeepClone(saveData);

            Assert.IsNotNull(new TemporalDifferenceQTrainerParameter(saveData));
            var copiedParameter = new TemporalDifferenceQTrainerParameter(copiedSaveData);
            
            chainer.Helper.AssertMatrixAlmostEqual(copiedParameter.State, parameter.State);
            chainer.Helper.AssertMatrixAlmostEqual(copiedParameter.NextState, parameter.NextState);
        }
    }
}