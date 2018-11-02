using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public struct MotionTargetSaveData : IMotionGeneratorSerializable<MotionTargetSaveData>
    {
        [Key(0)] public float Time { get; set; }
        [Key(1)] public float[] Values { get; set; }

        public MotionTargetSaveData(float time, float[] values)
        {
            Time = time;
            Values = values;
        }
    }
}