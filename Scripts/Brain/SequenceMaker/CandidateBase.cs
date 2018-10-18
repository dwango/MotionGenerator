using System;
using System.Collections.Generic;
using System.Linq;

namespace MotionGenerator
{
    public abstract class CandidateBase
    {
        // RandomSequenceMakerと同じ
        private static readonly List<float> InitialControlPointTimes = new List<float>{0.25f, 0.75f, 1f};
        
        const float InitialValue = 0.5f; // 値域が[0,1]なので中心の0.5
        const float InitialForceValue = 0.5f; // 値域が[0,1]なので中心の0.5
        
        protected Dictionary<Guid, MotionSequence> CopyValueWithSequenceMapping(
            Dictionary<Guid, MotionSequence> originalValue,
            Dictionary<Guid, int> newmanipulatableDimensions)
        {
            var manipulatableIds = newmanipulatableDimensions.Keys;
            var value = new Dictionary<Guid, MotionSequence>();
            var sequenceCount = newmanipulatableDimensions.Count;
            foreach (var manipulatableId in manipulatableIds)
            {
                MotionSequence motionSequence;
                if (!originalValue.ContainsKey(manipulatableId))
                {
                    // 親には存在しない新しいManipulatorなので、初期値を設定
                    var initialMotionTarget = new List<MotionTarget>();
                    foreach (var time in InitialControlPointTimes)
                    {
                        var initialValues = Enumerable.Repeat(InitialValue, newmanipulatableDimensions[manipulatableId]).ToList();
                        
                        // forceは0次元目という決め打ちなので
                        initialValues[0] = InitialForceValue;
                        
                        // TODO: RandomSequenceMakerに統合する。それまでは0.5秒で生成する
                        initialMotionTarget.Add(new MotionTarget(time * 0.5f, initialValues));
                    }
                    motionSequence = new MotionSequence(initialMotionTarget);
                }
                else
                {
                    // 親のManipulatorのSequenceをコピー
                    motionSequence = new MotionSequence(originalValue[manipulatableId]);
                }
                value.Add(manipulatableId, motionSequence);
            }

            return value;
        }
    }
}