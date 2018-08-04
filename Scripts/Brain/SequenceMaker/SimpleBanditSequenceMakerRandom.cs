using System.Collections.Generic;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class SimpleBanditSequenceMakerRandom : SimpleBanditSequenceMaker
    {
        public SimpleBanditSequenceMakerRandom(float epsilon, int minimumCandidates) : base(epsilon, minimumCandidates)
        {
        }

        public SimpleBanditSequenceMakerRandom(SimpleBanditSequenceMakerRandomSaveData saveData)
            : base(saveData.SimpleBandit)
        {
        }

        public new SimpleBanditSequenceMakerRandomSaveData Save()
        {
            return new SimpleBanditSequenceMakerRandomSaveData(
                base.Save()
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        protected override Candidate SelectByCuriosity(List<Candidate> candidates)
        {
            var index = RandomGenerator.Next(0, candidates.Count);
            return candidates[index];
        }
    }
}