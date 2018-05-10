using MathNet.Numerics.LinearAlgebra;
using MotionGenerator.Algorithm.Reinforcement.Models;

namespace MotionGenerator.Algorithm.Reinforcement.Samples
{
    /// <summary>
    /// 0,3,4,0,3,4...というactionをとるようになれば成功
    /// その場合、平均リワードは7 * 0.7 / 3(=epsilon) で1.8くらい
    /// </summary>
    public class SingleSoulSample : QLearningBenchmarkBase
    {
        private ModelBase model;
        private TemporalDifferenceQTrainer trainer;
        Matrix<float> state;
        private float lastReward;
        private int _times = 0;

        void Start()
        {
            model = new MotionGenerator.Algorithm.Reinforcement.Models.Simple4Layer(
                inputDimention: 1, outputDimention: 5, hiddenDimention: 30);
            trainer = new MotionGenerator.Algorithm.Reinforcement.TemporalDifferenceQTrainer(
                epsilon: 0.3f, qNetwork: model, historySize: 100000, discountRatio: 0.9f, actionDimention: 5,
                replaySize: 32, rewardWeights: new[] {1f});
            state = Matrix<float>.Build.DenseDiagonal(1, 0);
            lastReward = 0f;
        }

        void Update()
        {
            _times += 1;
            var action = trainer.Predict(state / 10, lastReward: new[] {lastReward}, forceRandom: false);
            lastReward = RewardFunction0(state, action);
            if (_times % 1 == 0)
            {
                UnityEngine.Debug.Log("state/action: " + state[0, 0] + "/" + action);
                UnityEngine.Debug.Log("reward: " + lastReward);
            }
            state = NextState(state, action);
            if (_times % 30 == (30 - 1))
            {
                trainer.TrainWithHistory();
            }
        }
    }
    

}