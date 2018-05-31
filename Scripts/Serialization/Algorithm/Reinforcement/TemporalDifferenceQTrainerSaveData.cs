using MessagePack;
using MotionGenerator.Algorithm.Reinforcement;
using Serialization;

namespace MotionGenerator.Serialization.Algorithm.Reinforcement
{
    [MessagePackObject]
    public class ParameterSaveData : IMotionGeneratorSerializable<ParameterSaveData>
    {
        [Key(0)]
        public float[] Rewards { get; set; }

        [Key(1)]
        public int Action { get; set; }

        [Key(2)]
        public float[,] State { get; set; }

        [Key(3)]
        public float[,] NextState { get; set; }

        public ParameterSaveData(float[] rewards, int action, float[,] state, float[,] nextState)
        {
            Rewards = rewards;
            Action = action;
            State = state;
            NextState = nextState;
        }

        public ParameterSaveData()
        {
        }

        public TemporalDifferenceQTrainerParameter Instantiate()
        {
            return new TemporalDifferenceQTrainerParameter(this);
        }
    }
}
