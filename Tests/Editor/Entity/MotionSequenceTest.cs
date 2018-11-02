using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using MotionGenerator.Serialization;
using UnityEngine;


namespace MotionGenerator
{
    public class MotionSequenceTest
    {
        [Test]
        public void MotionSequenceのシリアライズ()
        {
            var a = new MotionTarget(0.0f, new List<float> {0.0f, 1.0f, 2.0f});
            var b = new MotionTarget(1.0f, new List<float> {0.0f, 2.0f, 4.0f});

            var src = new MotionSequence(new List<MotionTarget> {a, b});
            var dst = new MotionSequence(MotionGeneratorSerialization.DeepClone(src.Save()));
            
            Assert.AreEqual(src.Sequences.Length, dst.Sequences.Length);
            for (int n = 0; n < src.Sequences.Length; ++n)
            {
                var s = src[n];
                var d = dst[n];
                Assert.AreEqual(s.Time, d.Time);
                Assert.AreEqual(s.Values.Length, d.Values.Length);
                for (int m = 0; m < s.Values.Length; ++m)
                {
                    Assert.AreEqual(s.Values[m], d.Values[m]);
                }
            }
        }
    }

}