﻿using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MotionGenerator.Algorithm.Reinforcement;
using MotionGenerator.Algorithm.Reinforcement.Models;
using MotionGenerator.Serialization;
using SimpleJSON;
using MotionGenerator.Serialization.Algorithm.Reinforcement;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator
{
    public class ReinforcementDecisionMaker : DecisionMakerBase
    {
//        private readonly KeyValueLogger _rewardLogger;
        private readonly int _historySize;
        private readonly float _discountRatio;
        private float[] _lastRewards = {0, 0};
        private ModelBase _model;

        private readonly float[] _soulWeights;
        private readonly float _optimizerAlpha;
        private readonly bool _enableRandomForgetting;

        private static readonly string[] DefaultKeyOrder =
        {
            State.BasicKeys.Position,
            State.BasicKeys.Forward,
            State.BasicKeys.RelativeFoodPosition,
            State.BasicKeys.RelativeCreaturePosition,
            State.BasicKeys.RelativeTribePosition,
            State.BasicKeys.RelativeStrangerPosition,
            State.BasicKeys.RelativeObjectPosition,
            State.BasicKeys.TotalFoodCountEachDirection,
            State.BasicKeys.TotalCreatureCountEachDirection,
            State.BasicKeys.TotalTribeCountEachDirection,
            State.BasicKeys.TotalStrangerCountEachDirection,
            State.BasicKeys.TotalObjectCountEachDirection,
            State.BasicKeys.RelativeObservedTilePosition,
        };

        private readonly string[] _keyOrder;

        private TemporalDifferenceQTrainer _trainer;
        private int _inputDimention;
        private readonly string _optimizerType;
        private readonly int _hiddenDimention;
        protected Dictionary<int, IDecisionMaker> SubDecisionMakers = new Dictionary<int, IDecisionMaker>();
        private List<ParameterSaveData> _historySaveData;
        private float _randomActionProbability;

        public bool ForDebugEnableAvoidTraining = true; // falseにすると全てのActionを学習対象にする

        private Matrix<float> State2Matrix(State state)
        {
            float[] vec;
            vec = _keyOrder
                .Where(key => state.ContainsKey(key))
                .SelectMany(key =>
                {
                    switch (key)
                    {
                        case State.BasicKeys.Position:
                            var pos = (state[key] - state[State.BasicKeys.BirthPosition]).ToSingle() / 101;
                            for (int i = 0; i < pos.Count; i++)
                            {
                                if (pos[i] > 1.0f)
                                {
                                    pos[i] = 1.0f;
                                }
                                else if (pos[i] < -1.0f)
                                {
                                    pos[i] = -1.0f;
                                }
                            }

                            return pos;
                        case State.BasicKeys.TotalFoodCountEachDirection:
                        case State.BasicKeys.TotalCreatureCountEachDirection:
                        case State.BasicKeys.TotalTribeCountEachDirection:
                        case State.BasicKeys.TotalStrangerCountEachDirection:
                        case State.BasicKeys.TotalObjectCountEachDirection:
                            var states = state[key].ToSingle();
                            for (var i = 0; i < states.Count; i++)
                            {
                                states[i] /= 100f;
                                if (states[i] > 1f)
                                {
                                    states[i] = 1f;
                                }
                                else if (states[i] < 0f)
                                {
                                    states[i] = 0f;
                                }
                            }

                            return states;
                        default:
                            return state[key].ToSingle();
                    }
                })
                .ToArray();
            return Matrix<float>.Build.DenseOfRowArrays(vec);
        }


        public ReinforcementDecisionMaker(int historySize = 100000, float discountRatio = 0.9f,
            float[] soulWeights = null, string optimizerType = "adam", int hiddenDimention = 32,
            string[] keyOrder = null, float optimizerAlpha = 0.01f, bool enableRandomForgetting = false)
        {
//            _rewardLogger = new KeyValueLogger(GetType().Name, key: "reward", id: GetHashCode().ToString());
            _historySize = historySize;
            _discountRatio = discountRatio;
            _soulWeights = soulWeights == null ? new float[] {1f} : soulWeights.ToArray();
            _optimizerType = optimizerType;
            _hiddenDimention = hiddenDimention;
            _optimizerAlpha = optimizerAlpha;
            _enableRandomForgetting = enableRandomForgetting;
            
            _randomActionProbability = 0.1f;
            if (keyOrder != null)
            {
                _keyOrder = keyOrder.ToArray();
            }
            else
            {
                _keyOrder = DefaultKeyOrder.ToArray();
            }
        }


        public ReinforcementDecisionMaker(ReinforcementDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
//            _rewardLogger = new KeyValueLogger(GetType().Name, key: "reward", id: GetHashCode().ToString());
            _historySize = saveData.HistorySize;
            _discountRatio = saveData.DiscountRatio;
            _lastRewards = saveData.LastRewards;
            _inputDimention = saveData.InputDimention;
            _soulWeights = saveData.SoulWeights;
            _optimizerType = saveData.OptimizerType;
            _hiddenDimention = saveData.HiddenDimention;
            _keyOrder = saveData.KeyOrder;
            _optimizerAlpha = saveData.OptimizerAlpha;
            _historySaveData = saveData.HistorySaveData;
            _randomActionProbability = saveData.RandomActionProbability;
            _enableRandomForgetting = saveData.EnableRandomForgetting;

            SubDecisionMakers = new Dictionary<int, IDecisionMaker>();
            for (int i = 0; i < saveData.SubDecisionMakersKeys.Length; i++)
            {
                SubDecisionMakers.Add(saveData.SubDecisionMakersKeys[i],
                    saveData.SubDecisionMakerValues[i].Instantiate());
            }

            var modelSaveDataJson = saveData.ModelSaveDataJson;
            if (modelSaveDataJson != null)
            {
                CreateModel();
                CreateTrainer(_inputDimention);
                var deserializer = new chainer.serializers.JsonDeserializer(JSON.Parse(modelSaveDataJson));
                _model.Serialize(deserializer);
            }
        }

        public new ReinforcementDecisionMakerSaveData Save()
        {
            string modelSaveDataJson = null;
            if (_model != null)
            {
                var serializer = new chainer.serializers.JsonSerializer();
                _model.Serialize(serializer);
                modelSaveDataJson = serializer.Fetch();
            }

            return new ReinforcementDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save(),
                _historySize,
                _discountRatio,
                _lastRewards,
                modelSaveDataJson,
                _inputDimention,
                _soulWeights,
                _optimizerType,
                _hiddenDimention,
                SubDecisionMakers.Keys.ToArray(),
                SubDecisionMakers.Values.Select(x => x.SaveAsInterface()).ToArray(),
                _keyOrder,
                _optimizerAlpha,
                _trainer != null
                    ? _trainer.GetHistorySaveData()
                    : new List<ParameterSaveData>(), //FIXME(kogaki) trainerのライフサイクル設計
                _randomActionProbability,
                _enableRandomForgetting
            );
        }

        public override DecisionMakerSaveData SaveAsInterface()
        {
            return new DecisionMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        private void LoadInitialModel(int inputDimention = 6)
        {
            _inputDimention = inputDimention;
            CreateModel();
            CreateTrainer(inputDimention);
//            if (Actions.Count == 8 && _inputDimention == 6)
//            {
//                var textAsset = Resources.Load("initial_weight_4layers") as TextAsset;
//                var json = SimpleJSON.JSON.Parse(textAsset.text);
//                var deserializer = new chainer.serializers.JsonDeserializer(json);
//                _model.Serialize(deserializer);
//            }
        }


        private void CreateModel()
        {
            _model = new Simple4LayerSigmoid(inputDimention: _inputDimention,
                outputDimention: Actions.Count * _soulWeights.Length, hiddenDimention: _hiddenDimention);
        }


        private void CreateTrainer(int inputDimention)
        {
            if (_inputDimention != inputDimention)
            {
                _inputDimention = inputDimention;
                _model.AlterInputDimention(inputDimention);
            }

            _trainer = new MotionGenerator.Algorithm.Reinforcement.TemporalDifferenceQTrainer(
                epsilon: _randomActionProbability, qNetwork: _model,
                historySize: _historySize, discountRatio: _discountRatio, actionDimention: Actions.Count,
                replaySize: 32, alpha: _optimizerAlpha, rewardWeights: _soulWeights, optimizerType: _optimizerType,
                enableRandomForgetting: _enableRandomForgetting,
                initialHistory: _historySaveData != null
                    ? _historySaveData.Select(x => x.Instantiate()).ToList()
                    : null
            );

            _historySaveData = null; //FIXME(kogaki): _historySaveDataをインスタンス変数に持たないようにしたい
        }

        public override void Init(List<IAction> actions)
        {
            base.Init(actions);

            if (_model == null)
            {
                LoadInitialModel();
            }

            // Instantiate subDMs
            var nonDecisionMakerActions = actions.Where(x => !(x is SubDecisionMakerAction)).ToList();
            var subDecisionMakers = new Dictionary<int, IDecisionMaker>();
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is SubDecisionMakerAction)
                {
                    subDecisionMakers.Add(i, (actions[i] as SubDecisionMakerAction).DecisionMaker);
                    subDecisionMakers[i].Init(nonDecisionMakerActions);
                }
            }

            SubDecisionMakers = subDecisionMakers;
        }

        public override void Init(IDecisionMaker parent)
        {
            var parentDecisionMaker = (ReinforcementDecisionMaker) parent;
            Init(parentDecisionMaker.Actions);

            CreateTrainer(parentDecisionMaker._inputDimention);

            var serializer = new chainer.serializers.DictionarySerializer();
            parentDecisionMaker._model.Serialize(serializer);
            var savedata = serializer.Target;
            var deserializer = new chainer.serializers.DictionaryDeserializer(savedata);
            _model.Serialize(deserializer);
        }

        public override void Restore(List<IAction> actions)
        {
            base.Restore(actions);

            // restore subDMs
            var nonDecisionMakerActions = actions.Where(x => !(x is SubDecisionMakerAction)).ToList();
            foreach (var subDM in SubDecisionMakers)
            {
                subDM.Value.Restore(nonDecisionMakerActions);
            }
        }

        public override void AlterSoulWeights(float[] soulWeights)
        {
            _trainer.AlterRewardWeights(soulWeights);
        }

        public override void SetRandomActionProbability(float probability)
        {
            _randomActionProbability = probability;
            _trainer.SetEpsilon(_randomActionProbability);
        }

        public override void ResetModel()
        {
            CreateModel();
            CreateTrainer(_inputDimention);
        }

        protected IAction ForceAction(State state, int actionIndex)
        {
            Assert.IsTrue(actionIndex < Actions.Count);
            return DecideAction(state, forceRandom: false, forceMax: false, forceAction: actionIndex);
        }

        public IAction DecideAction(State state, bool forceRandom, bool forceMax, int forceAction = -1)
        {
            var stateMarix = State2Matrix(state);
            if (stateMarix.ColumnCount != _inputDimention)
            {
                CreateTrainer(stateMarix.ColumnCount);
            }

            var avoidLearning = ForDebugEnableAvoidTraining &&
                                state.ContainsKey(State.BasicKeys.AvoidLearning) &&
                                (state.GetAsFloat(State.BasicKeys.AvoidLearning) > 0.0001f);

//            if (_rewardLogger.Enabled() && !avoidLearning)
//            {
//                _rewardLogger.Log(string.Join(",", _lastRewards.Select(x => x.ToString()).ToArray()));
//            }

            var index = _trainer.Predict(stateMarix, _lastRewards, forceRandom, forceMax, forceAction, avoidLearning);

            if (SubDecisionMakers.ContainsKey(index))
            {
                return SubDecisionMakers[index].DecideAction(state);
            }

            return Actions[index];
        }

        public override IAction DecideAction(State state)
        {
            return DecideAction(state, false, false);
        }

        private int _iteration = 0;

        public override void Feedback(List<float> reward)
        {
            _iteration += 1;
            _lastRewards = reward.ToArray();
            if (_iteration % 3 == 1)
            {
                _trainer.TrainWithHistory();
            }

//            if (_iteration % 10000 == 1)
//            {
//                var serializer = new chainer.serializers.JsonSerializer();
//                _model.Serialize(serializer);
//                var json = serializer.Fetch();
//                var path = _datapath + "/" + GetHashCode() + ".json";
//                var fi = new FileInfo(path);
////                UnityEngine.Debug.Log($"save to {path}");
//                var sw = fi.AppendText();
//                sw.Write(json);
//                sw.Flush();
//                sw.Close();
//            }
        }
    }
}