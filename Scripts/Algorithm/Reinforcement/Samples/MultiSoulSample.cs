using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MotionGenerator.Algorithm.Reinforcement.Models;

namespace MotionGenerator.Algorithm.Reinforcement.Samples
{
    public class MultiSoulSample : QLearningBenchmarkBase
    {
        public bool IsMultiSoulModel = true;
        private ModelBase model;
        private TemporalDifferenceQTrainer trainer;
        Matrix<float> state;
        private readonly float[] _lastReward = new[] {0f, 0f};
        private readonly float[] _soulWeights = new[] {0.1f, 0.9f};
        private int _times = 0;
        private TSVLogger _logger;

        void Start()
        {
            _logger = new TSVLogger();

            if (IsMultiSoulModel)
            {
                model = new MotionGenerator.Algorithm.Reinforcement.Models.Simple4Layer(
                    inputDimention: 1, outputDimention: 5 * _soulWeights.Length, hiddenDimention: 30 * 2);
                trainer = new MotionGenerator.Algorithm.Reinforcement.TemporalDifferenceQTrainer(
                    epsilon: 0.3f, qNetwork: model, historySize: 100000, discountRatio: 0.9f, actionDimention: 5,
                    replaySize: 32, rewardWeights: _soulWeights, alpha: 0.01f);
            }
            else
            {
                model = new MotionGenerator.Algorithm.Reinforcement.Models.Simple4Layer(
                    inputDimention: 1, outputDimention: 5, hiddenDimention: 30 * 2);
                trainer = new MotionGenerator.Algorithm.Reinforcement.TemporalDifferenceQTrainer(
                    epsilon: 0.3f, qNetwork: model, historySize: 100000, discountRatio: 0.9f, actionDimention: 5,
                    replaySize: 32, rewardWeights: new[] {1f}, alpha: 0.01f);
            }

            state = Matrix<float>.Build.DenseDiagonal(1, 0);
        }

        void Update()
        {
            _times += 1;
            int action;
            if (IsMultiSoulModel)
            {
                action = trainer.Predict(state / 10, lastReward: _lastReward, forceRandom: false);
            }
            else
            {
                var lastTotalReward = _lastReward.Select((t, i) => t * _soulWeights[i]).Sum();
                action = trainer.Predict(state / 10, lastReward: new[] {lastTotalReward}, forceRandom: false);
            }

            _lastReward[0] = RewardFunction0(state, action);
            _lastReward[1] = RewardFunction1(state, action);
            var totalReward = _lastReward.Select((t, i) => t * _soulWeights[i]).Sum();

            // 時々、どっちのSoulを優先するか入れ替える
            if (_times % 5000 == 1)
            {
                for (int i = 0; i < _soulWeights.Length; i++)
                {
                    if (_soulWeights[i] > 0.5f)
                    {
                        _soulWeights[i] = 0.1f;
                    }
                    else
                    {
                        _soulWeights[i] = 0.9f;
                    }
                }

                UnityEngine.Debug.Log("Time: " + _times);
                UnityEngine.Debug.Log("weights: " + string.Join(",", _soulWeights.Select(x => x.ToString()).ToArray()));
                if (IsMultiSoulModel)
                {
                    trainer.AlterRewardWeights(_soulWeights);
                }
            }

            if (_times % 1 == 0)
            {
                _logger.Write(new object[]
                {
                    "reward",
                    _lastReward[0],
                    _lastReward[1],
                    totalReward
                });
            }

            state = NextState(state, action);
            if (_times % 30 == (30 - 1))
            {
                trainer.TrainWithHistory();
            }
        }
    }
}