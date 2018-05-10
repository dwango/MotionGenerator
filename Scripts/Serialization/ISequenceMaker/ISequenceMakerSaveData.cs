using MessagePack;

namespace MotionGenerator.Serialization
{
    [Union(0, typeof(SequenceMakerBaseSaveData))]
    [Union(1, typeof(EvolutionarySequenceMakerSaveData))]
    [Union(2, typeof(FixedSequenceMakerSaveData))]
    [Union(3, typeof(LocomotionSequenceMakerSaveData))]
    [Union(4, typeof(RandomSequenceMaker))]
    [Union(5, typeof(SimpleBanditSequenceMakerSaveData))]
    [Union(6, typeof(SimpleBanditSequenceMakerRandomSaveData))]
    public interface ISequenceMakerSaveData
    {
        ISequenceMaker Instantiate();
    }
}
