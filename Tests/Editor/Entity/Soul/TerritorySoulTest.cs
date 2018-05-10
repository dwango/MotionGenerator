using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;
using UnityEngine;

namespace MotionGenerator.Entity.Soul
{
    public class TerritorySoulTest
    {
        [Test]
        public void クローンしたSoulが同じ報酬を得られる()
        {
            var originalSoul = new TerritorySoul(new Vector3(1, 2, 3));
            var childSoul = originalSoul.SaveAsInterface().Instantiate();

            var currentState = new State(new Dictionary<string, Vector>()
            {
                {State.BasicKeys.Position, new DenseVector(new double[] {0, 0, 0})}
            });
            foreach (var state in new[]
            {
                new State(new Dictionary<string, Vector>()
                {
                    {State.BasicKeys.Position, new DenseVector(new double[] {1, 2, 3})}
                }),
                new State(new Dictionary<string, Vector>()
                {
                    {State.BasicKeys.Position, new DenseVector(new double[] {2, 2, 2})}
                }),
            })
            {
                var lastState = currentState;
                currentState = state;
                Assert.AreEqual(
                    originalSoul.Reward(lastState, currentState),
                    childSoul.Reward(lastState, currentState)
                );
            }
        }
    }
}