using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SimpleBanditSequenceMakerRandomSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<SimpleBanditSequenceMakerRandomSaveData>
    {
        [Key(0)] public SimpleBanditSequenceMakerSaveData SimpleBandit { get; set; }

        public SimpleBanditSequenceMakerRandomSaveData()
        {
            
        }

        public SimpleBanditSequenceMakerRandomSaveData(SimpleBanditSequenceMakerSaveData simpleBandit)
        {
            SimpleBandit = simpleBandit;
        }

        public SimpleBanditSequenceMaker Instantiate()
        {
            return new SimpleBanditSequenceMakerRandom(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}