using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public struct MotionTarget
    {
        public float Time;
        public readonly float[] Values;

        public MotionTarget(float time, List<float> values)
        {
            Time = time;
            Values = values.ToArray();
        }

        public MotionTarget(float time, float[] values)
        {
            Time = time;
            Values = values;
        }

        public MotionTarget(MotionTarget origin)
        {
            Time = origin.Time;
            Values = origin.Values.ToArray();
        }

        public MotionTarget(MotionTargetSaveData saveData)
        {
            Time = saveData.Time;
            Values = saveData.Values;
        }

        public MotionTargetSaveData Save()
        {
            return new MotionTargetSaveData(
                Time,
                Values
            );
        }
    }

    public class MotionSequence
    {
        public readonly MotionTarget[] Sequences;
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

        public MotionSequence(MotionSequence origin)
        {
            Sequences = origin.Sequences.Select(x => new MotionTarget(x)).ToArray(); // DeepClone
            _index = 0;
        }

        public MotionSequence() : this(new MotionTarget[0])
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

        public MotionTarget this[int i] => Sequences[i];

        public MotionTarget CurrentSequence()
        {
            return Sequences[_index];
        }

        public bool Next()
        {
            ++_index;
            return _index != Sequences.Length;
        }


        public float GetDuration()
        {
            return Sequences[Sequences.Length - 1].Time;
        }
    }
}