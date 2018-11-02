﻿using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public struct MotionTarget
    {
        public float Time;
        public float[] Values;

        public MotionTarget(float time, List<float> values)
        {
            Time = time;
            Values = values.ToArray();
        }

        public MotionTarget(float time, float[] values)
        {
            Time = time;
            Values = values.ToArray();
        }

        public MotionTarget(MotionTarget origin)
        {
            Time = origin.Time;
            Values = origin.Values.ToArray();
        }

        public MotionTarget(MotionTargetSaveData saveData)
        {
            Time = saveData.Time;
            Values = saveData.Values.ToArray();
        }

        public MotionTargetSaveData Save()
        {
            return new MotionTargetSaveData(
                Time,
                Values.ToArray()
            );
        }
    }

    public class MotionSequence
    {
        public MotionTarget[] Sequences;
        private int _index;

        public MotionSequence(List<MotionTarget> sequences)
        {
            Sequences = sequences.ToArray();
            _index = 0;
        }

        public MotionSequence(MotionTarget[] sequences)
        {
            Sequences = sequences;
            _index = 0;
        }

        public MotionSequence(MotionSequence motionSequence)
        {
            Sequences = motionSequence.Sequences.ToArray();
            _index = 0;
        }

        public MotionSequence()
            : this(new List<MotionTarget>())
        {
        }

        public MotionSequence(MotionSequenceSaveData saveData)
        {
            Sequences = saveData.Sequences.Select(x => new MotionTarget(x)).ToArray();
            _index = saveData.Index;
        }

        public MotionSequenceSaveData Save()
        {
            return new MotionSequenceSaveData(
                Sequences.Select(x => x.Save()).ToArray(),
                _index
            );
        }

        public MotionTarget this[int i]
        {
            get { return Sequences[i]; }
        }
        
        public MotionTarget CurrentSequence()
        {
            return Sequences[_index];
        }

        public bool Next()
        {
            ++_index;
            return _index == Sequences.Length ? false : true;
        }
        

        public float GetDuration()
        {
            return Sequences[Sequences.Length - 1].Time;
        }
    }
}