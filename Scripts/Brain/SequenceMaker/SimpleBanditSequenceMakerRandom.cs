using System.Collections.Generic;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class SimpleBanditSequenceMakerRandom : SimpleBanditSequenceMaker
    {
        private const string ThisTypeString = "SimpleBanditSequenceMakerRandom";
        static SimpleBanditSequenceMakerRandom()
        {
            SequenceMakerSaveData.AddDeserializer(ThisTypeString, baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<SimpleBanditSequenceMakerRandomSaveData>(baseData);
                return new SimpleBanditSequenceMakerRandom(saveData);
            });
        }
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
            return new SequenceMakerSaveData(ThisTypeString, MotionGeneratorSerialization.Serialize(Save()));
        }

        protected override Candidate SelectByCuriosity(List<Candidate> candidates)
        {
            var index = RandomGenerator.Next(0, candidates.Count);
            return candidates[index];
        }
    }
}