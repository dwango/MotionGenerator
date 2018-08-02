using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Entity.Soul;
using NUnit.Framework;
using UnityEngine;

namespace MotionGenerator.Tests.Editor.Brain
{
    public class BrainIntegrationTest
    {
        [Test]
        public void JustSampleUsage()
        {
            var actions = LocomotionAction.EightDirections();
            var sequenceMaker = new EvolutionarySequenceMaker(epsilon: 0.3f, minimumCandidates: 30);
            var decisionMaker = new ReinforcementDecisionMaker();
            var brain = new MotionGenerator.Brain(
                decisionMaker,
                sequenceMaker
            );
            brain.Init(
                new List<int> {(new ManipulatableMock()).GetManipulatableDimention()},
                actions,
                new List<ISoul>() {new GluttonySoul()}
            );


            // Position, Rotation is required to update LocomotionAction
            // GluttonySoul.Key is required to update GluttonySoul
            // others are used to decide action
            brain.GenerateMotionSequence(new State(new Dictionary<string, Vector>()
            {
                {State.BasicKeys.RelativeFoodPosition, DenseVector.OfArray(new double[]{0.5f, 0.5f, 0.5f})},
                {State.BasicKeys.BirthPosition, DenseVector.OfArray(new double[]{0f, 0f, 0f})},
                {State.BasicKeys.Position, DenseVector.OfArray(new double[]{0f, 0f, 0f})},
                {State.BasicKeys.Rotation, DenseVector.OfArray(new double[]{0f, 0f, 0f, 0f})},
                {GluttonySoul.Key, DenseVector.OfArray(new double[]{0f})}
            }));
            
            brain.GenerateMotionSequence(new State(new Dictionary<string, Vector>()
            {
                {State.BasicKeys.RelativeFoodPosition, DenseVector.OfArray(new double[]{0.5f, 0.5f, 0.5f})},
                {State.BasicKeys.BirthPosition, DenseVector.OfArray(new double[]{0f, 0f, 0f})},
                {State.BasicKeys.Position, DenseVector.OfArray(new double[]{0.1f, 0f, 0f})},
                {State.BasicKeys.Rotation, DenseVector.OfArray(new double[]{0f, 0f, 0f, 0f})},
                {GluttonySoul.Key, DenseVector.OfArray(new double[]{1f})}
            }));            
        }
    }
}