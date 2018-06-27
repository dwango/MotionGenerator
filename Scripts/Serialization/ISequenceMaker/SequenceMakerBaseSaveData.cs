using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SequenceMakerBaseSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<SequenceMakerBaseSaveData>
    {
        public SequenceMakerBaseSaveData()
        {
            
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}