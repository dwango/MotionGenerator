using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using BodyGenerator.Body.Manipulatables;

namespace MotionGenerator
{
    public class JointTest
    {
        // TODO: Test with Scene

        [Test]
        public void JustConstructBendJoint1DTest()
        {
            var joint = new GameObject("test", typeof(BendJoint1D)).GetComponent<BendJoint1D>();
            var sequence = new MotionSequence(new List<MotionTarget>
            {
                new MotionTarget(
                    20,
                    new List<float> {1, 1, 1}
                )
            });
            joint.Manipulate(sequence);
        }

        [Test]
        public void JustConstructBendJoint2DTest()
        {
            var joint = new GameObject("test", typeof(BendJoint2D)).GetComponent<BendJoint2D>();
            var sequence = new MotionSequence(new List<MotionTarget>
            {
                new MotionTarget(
                    20,
                    new List<float> {1, 1, 1}
                )
            });
            joint.Manipulate(sequence);
        }

        [Test]
        public void JustConstructSpringJointTest()
        {
            var joint = new GameObject("test", typeof(BodyGenerator.Body.Manipulatables.SpringJoint))
                .GetComponent<BodyGenerator.Body.Manipulatables.SpringJoint>();
            var sequence = new MotionSequence(new List<MotionTarget>
            {
                new MotionTarget(
                    20,
                    new List<float> {1, 1, 1}
                )
            });
            joint.Manipulate(sequence);
        }
    }
}