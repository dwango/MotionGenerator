using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public struct MotionSequenceSaveData : IMotionGeneratorSerializable<MotionSequenceSaveData>
    {
        [Key(0)] public MotionTargetSaveData[] Sequences { get; set; }
        [Key(1)] public int Index { get; set; }

        public MotionSequenceSaveData(MotionTargetSaveData[] sequences, int index)
        {
            Sequences = sequences;
            Index = index;
        }
    }
}