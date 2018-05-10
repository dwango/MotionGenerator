using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace MotionGenerator
{
    public class StateTest
    {
        [Test]
        public void Stateのマージ()
        {
            var state_a = new State();
            state_a["key0"] = new DenseVector(1);
            state_a["key0"][0] = 123;
            state_a["key1"] = new DenseVector(2);
            state_a["key1"][0] = 1234;
            state_a["key1"][1] = 12345;

            var state_b = new State();
            state_b["key1"] = new DenseVector(10);
            state_b["key2"] = new DenseVector(20);

            state_b.MergeFrom(state_a);

            Assert.AreEqual(state_b.Count, 3);
            Assert.AreEqual(state_b["key0"].Count, 1);
            Assert.AreEqual(state_b["key0"][0], 123);
            Assert.AreEqual(state_b["key1"].Count, 2);
            Assert.AreEqual(state_b["key1"][0], 1234);
            Assert.AreEqual(state_b["key1"][1], 12345);
            Assert.AreEqual(state_b["key2"].Count, 20);
        }


        [Test]
        public void Stateの置き換え()
        {
            var state_a = new State();
            state_a["key0"] = new DenseVector(1);
            state_a["key0"][0] = 123;
            state_a["key1"] = new DenseVector(2);
            state_a["key1"][0] = 1234;
            state_a["key1"][1] = 12345;

            var state_b = new State();
            state_b["key1"] = new DenseVector(10);
            state_b["key2"] = new DenseVector(20);

            state_b.ReplaceFrom(state_a);

            Assert.AreEqual(state_b.Count, 2);
            Assert.AreEqual(state_b["key0"].Count, 1);
            Assert.AreEqual(state_b["key0"][0], 123);
            Assert.AreEqual(state_b["key1"].Count, 2);
            Assert.AreEqual(state_b["key1"][0], 1234);
            Assert.AreEqual(state_b["key1"][1], 12345);
        }
    }
}