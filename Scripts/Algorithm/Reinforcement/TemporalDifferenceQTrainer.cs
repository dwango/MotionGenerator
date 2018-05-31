using System;
using System.Collections.Generic;
using System.Linq;
using chainer;
using MathNet.Numerics.LinearAlgebra;
using MotionGenerator.Serialization.Algorithm.Reinforcement;
using UnityEngine.Assertions;

namespace MotionGenerator.Algorithm.Reinforcement
{
    public class TemporalDifferenceQTrainerParameter
    {
        public readonly float[] Rewards;
        public readonly int Action;
        public readonly Matrix<float> State;
        public readonly Matrix<float> NextState;

        public TemporalDifferenceQTrainerParameter(float[] rewards, int action, Matrix<float> state,
            Matrix<float> nextState)
        {
            Rewards = new float[rewards.Length];
            rewards.CopyTo(Rewards, 0);
            Action = action;
            State = state;
            NextState = nextState;
        }

        public TemporalDifferenceQTrainerParameter(ParameterSaveData saveData)
        {
            Rewards = saveData.Rewards;
            Action = saveData.Action;
            Assert.IsNotNull(saveData.State);
            State = MathNet.Numerics.LinearAlgebra.Single.DenseMatrix.OfArray(saveData.State);
            NextState = MathNet.Numerics.LinearAlgebra.Single.DenseMatrix.OfArray(saveData.NextState);
        }

        public ParameterSaveData Save()
        {
            return new ParameterSaveData(rewards: Rewards, action: Action, state: State.ToArray(),
                nextState: NextState.ToArray());
        }
    }

    public class TemporalDifferenceQTrainer
    {
        private readonly double _epsilon;
        private readonly Chain _qNetwork;
        private readonly int _historySize;
        private readonly int _replaySize;
        private readonly float _discountRatio;
        private readonly int _actionDimention;
        private float[] _rewardWeights;

        public float[] LastDecisionWeightsForViewer { get; private set; }
//        private readonly KeyValueLogger _qValueLogger;

        private readonly System.Random _random = new System.Random();

        private readonly List<TemporalDifferenceQTrainerParameter> _history =
            new List<TemporalDifferenceQTrainerParameter>();

        private int _lastAction = 0;
        private Matrix<float> _lastState = null;

        private readonly chainer.optimizers.Optimizer _optimizer;

        public TemporalDifferenceQTrainer(double epsilon, Chain qNetwork, int historySize, float discountRatio,
            int actionDimention, int replaySize, float[] rewardWeights, float alpha = 0.001f,
            string optimizerType = "adam", List<TemporalDifferenceQTrainerParameter> initialHistory = null)
        {
            _epsilon = epsilon;
            _qNetwork = qNetwork;
            _historySize = historySize;
            Assert.IsTrue(0f <= discountRatio && discountRatio < 1f);
            _discountRatio = discountRatio;
            _actionDimention = actionDimention;
            _replaySize = replaySize;
            _rewardWeights = rewardWeights;
//            _qValueLogger = new KeyValueLogger(GetType().Name, key: "qvalues", id: GetHashCode().ToString());
            LastDecisionWeightsForViewer = rewardWeights.Select(_ => 0f).ToArray();
            switch (optimizerType)
            {
                case "adam":
                    _optimizer = new chainer.optimizers.Adam(alpha: alpha);
                    break;
                case "sgd":
                    _optimizer = new chainer.optimizers.SGD(lr: alpha);
                    break;
                default:
                    throw new Exception("no such optimizer");
            }

            _optimizer.Setup(qNetwork);
            _history = initialHistory ?? new List<TemporalDifferenceQTrainerParameter>();
        }

        public TemporalDifferenceQTrainer(double epsilon, Chain qNetwork, int historySize, float discountRatio,
            int actionDimention, float[] rewardWeights) : this(epsilon, qNetwork, historySize, discountRatio,
            actionDimention, rewardWeights: rewardWeights, replaySize: historySize)
        {
        }

        private int PredictRandom(Matrix<float> state)
        {
            return _random.Next(_actionDimention);
        }

        private int PredictMax(Matrix<float> state)
        {
            var qValue = _qNetwork.Forward(new Variable(state));
            var qValueVector = qValue.Value.Clone().Row(0);
            qValue.VolatilizeWithoutBackward();

//            if (_qValueLogger.Enabled())
//            {
//                _qValueLogger.Debug("state:" + string.Join(",", state.Row(0).Select(x => x.ToString()).ToArray()));
//                _qValueLogger.Debug(string.Join(",", qValueVector.Select(x => x.ToString()).ToArray()));
//            }

            var mergedValue = Enumerable.Repeat(0f, _actionDimention).ToArray();
            for (int i_reward = 0; i_reward < _rewardWeights.Length; i_reward++)
            {
                LastDecisionWeightsForViewer[i_reward] = 0;
                var weight = _rewardWeights[i_reward];
                for (int i_action = 0; i_action < _actionDimention; i_action++)
                {
                    // i_soul * action数 + i_actionが、そのsoulそのactionの推測値
                    // 全soulのそのaction取りたさの和を取る
                    // TODO(ogaki): 和じゃなくてmaxでもいいかも
                    var prediceted = qValueVector[i_action + _actionDimention * i_reward] * weight;
                    mergedValue[i_action] += prediceted;
                    LastDecisionWeightsForViewer[i_reward] = Math.Max(
                        prediceted, LastDecisionWeightsForViewer[i_reward]);
                }
            }

            // 表示用の予測値を正規化
            var maxDecisionWeight = LastDecisionWeightsForViewer.Sum();
            for (int i_reward = 0; i_reward < _rewardWeights.Length; i_reward++)
            {
                LastDecisionWeightsForViewer[i_reward] /= maxDecisionWeight;
            }

            var maxIndex = mergedValue
                .Select((value, index) => new {index, value})
                .OrderByDescending(item => item.value)
                .First()
                .index;
            return maxIndex;
        }

        public int Predict(Matrix<float> state, float[] lastReward, bool forceRandom = false, bool forceMax = false,
            int forceAction = -1, bool avoidLearning = false)
        {
            // decide action
            int action;
            if (forceAction < 0)
            {
                if (!forceMax && (forceRandom || _random.NextDouble() < _epsilon))
                {
                    action = PredictRandom(state);
                }
                else
                {
                    action = PredictMax(state);
                }
            }
            else
            {
                action = forceAction;
            }

            // update history
            if (_lastState != null && !avoidLearning)
            {
                _history.Add(
                    new TemporalDifferenceQTrainerParameter(rewards: lastReward, action: _lastAction, state: _lastState,
                        nextState: state));
            }

            while (_history.Count > _historySize)
            {
                _history.RemoveAt(0);
            }

            _lastAction = action;
            _lastState = state;
            return action;
        }

        public void AlterRewardWeights(float[] rewardWeights)
        {
            Assert.AreEqual(rewardWeights.Length, _rewardWeights.Length);
            rewardWeights.CopyTo(_rewardWeights, 0);
        }

        /// <summary>
        /// Train with Experience Replay
        /// </summary>
        public void TrainWithHistory()
        {
            Variable loss = new Variable(Matrix<float>.Build.DenseDiagonal(1, 0));
            for (int i = 0; i < System.Math.Min(_replaySize, _history.Count); i++)
            {
                var parameter = _history[_random.Next(_history.Count)];

                // next predicted reward
                var nextPredictedRewardtmp = _qNetwork.Forward(new Variable(parameter.NextState));
                var nextPredictedReward = new Variable(nextPredictedRewardtmp.Value.Clone());
                nextPredictedRewardtmp.VolatilizeWithoutBackward();


                // current predicted reward
                var predictedReward = _qNetwork.Forward(new Variable(parameter.State));

                // real reward by each soul
                var dummyTargetValue = predictedReward.Value.Clone();
                for (int i_soul = 0; i_soul < parameter.Rewards.Length; i_soul++)
                {
                    var nextMaxRewardInSoul = 0f;
                    for (int i_action = 0; i_action < _actionDimention; i_action++)
                    {
                        nextMaxRewardInSoul = Math.Max(nextMaxRewardInSoul,
                            nextPredictedReward.Value[0, i_action + i_soul * _actionDimention]);
                    }

                    var reward = parameter.Rewards[i_soul];
                    var targetPrediction = reward + nextMaxRewardInSoul * _discountRatio;

                    // i_soul * action数 + i_actionが、そのsoulそのactionの推測値
                    var prediction = predictedReward.Value[0, parameter.Action + i_soul * _actionDimention];
                    // 目標値 - 予測値 = 修正誤差
                    var rawDifference = targetPrediction - prediction;
//                    if (_qValueLogger.Enabled())
//                    {
//                        _qValueLogger.Debug("diff:" + rawDifference + "/reward" + reward);
//                    }

                    // lossのためのdummy値なので、性格で重み付けする
                    dummyTargetValue[0, parameter.Action + i_soul * _actionDimention] =
                        prediction + rawDifference * _rewardWeights[i_soul];
                }

                loss = loss + chainer.functions.MeanSquaredError.ForwardStatic(
                           predictedReward,
                           new Variable(dummyTargetValue)
                       );
            }

            _optimizer.ZeroGrads();
            loss.Backward();
            _optimizer.Update();
        }

        public List<ParameterSaveData> GetHistorySaveData()
        {
            return _history.Select(x => x.Save()).ToList();
        }
    }
}
