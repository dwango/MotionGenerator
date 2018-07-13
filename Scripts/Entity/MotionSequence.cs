using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class MotionTarget
    {
        public float time;
        public List<float> value;

        public MotionTarget(float time, List<float> value)
        {
            this.time = time;
            this.value = value;
        }

        public MotionTarget(MotionTargetSaveData saveData)
        {
            time = saveData.Time;
            value = saveData.Value;
        }

        public MotionTargetSaveData Save()
        {
            return new MotionTargetSaveData(
                time,
                value
            );
        }
    }

    public class MotionSequence
    {
        public List<MotionTarget> Sequence;

        public MotionSequence(List<MotionTarget> sequence)
        {
            Sequence = sequence;
        }

        public MotionSequence(MotionSequence motionSequence)
        {
            Sequence = new List<MotionTarget>(motionSequence.Sequence);
        }

        public MotionSequence()
            : this(new List<MotionTarget>())
        {
        }

        public MotionSequence(MotionSequenceSaveData saveData)
        {
            Sequence = saveData.Sequence.Select(x => new MotionTarget(x)).ToList();
        }

        public MotionSequenceSaveData Save()
        {
            return new MotionSequenceSaveData(
                Sequence.Select(x => x.Save()).ToList()
            );
        }

        public MotionTarget this[int i]
        {
            get
            {
                return Sequence [i];
            }
        }

        public float GetDuration()
        {
            return Sequence.Last().time;
        }
    }
}