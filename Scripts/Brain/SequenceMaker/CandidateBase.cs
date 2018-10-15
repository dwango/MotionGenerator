using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public abstract class CandidateBase
    {
        // RandomSequenceMakerと同じ
        private static readonly List<float> InitialControlPointTimes = new List<float>{0.25f, 0.75f, 1f};
        
        const float InitialValue = 0.5f; // 値域が[0,1]なので中心の0.5
        const float InitialForceValue = 0.5f; // 値域が[0,1]なので中心の0.5
        
        protected List<MotionSequence> CopyValueWithSequenceMapping(List<MotionSequence> originalValue, List<int> manipulationDimensions, Dictionary<int, int> childSequenceIdToParentSequenceId)
        {
            var value = new List<MotionSequence>();
            var sequenceCount = manipulationDimensions.Count;
            for (var sequenceId = 0; sequenceId < sequenceCount; sequenceId++)
            {
                var referenceSequenceId = childSequenceIdToParentSequenceId[sequenceId];
                MotionSequence motionSequence = null;
                if (referenceSequenceId == -1)
                {
                    // 親には存在しない新しいManipulatorなので、初期値を設定
                    var initialMotionTarget = new List<MotionTarget>();
                    foreach (var time in InitialControlPointTimes)
                    {
                        var initialValues = Enumerable.Repeat(InitialValue, manipulationDimensions[sequenceId]).ToList();
                        
                        // forceは0次元目という決め打ちなので
                        initialValues[0] = InitialForceValue;
                        
                        initialMotionTarget.Add(new MotionTarget(time, initialValues));
                    }
                    motionSequence = new MotionSequence(initialMotionTarget);
                }
                else
                {
                    // 親のManipulatorのSequenceをコピー
                    motionSequence = new MotionSequence(originalValue[referenceSequenceId]);
                }
                value.Add(motionSequence);
            }

            return value;
        }
    }
}