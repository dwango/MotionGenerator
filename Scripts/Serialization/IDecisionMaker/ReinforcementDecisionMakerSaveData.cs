using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization.Algorithm.Reinforcement;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ReinforcementDecisionMakerSaveData : IDecisionMakerSaveData, IMotionGeneratorSerializable<ReinforcementDecisionMakerSaveData>
    {
        [Key(0)] public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }
        [Key(1)] public int HistorySize { get; set; }
        [Key(2)] public float DiscountRatio { get; set; }
        [Key(3)] public float[] LastRewards { get; set; }
        [Key(4)] public string ModelSaveDataJson { get; set; }
        [Key(5)] public int InputDimention { get; set; }
        [Key(6)] public float[] SoulWeights { get; set; }
        [Key(7)] public string OptimizerType { get; set; }
        [Key(8)] public int HiddenDimention { get; set; }
        [Key(9)] public int[] SubDecisionMakersKeys { get; set; }
        [Key(10)] public IDecisionMakerSaveData[] SubDecisionMakerValues { get; set; }
        [Key(11)] public string[] KeyOrder { get; set; }
        [Key(12)] public float OptimizerAlpha { get; set; }
        [Key(13)] public List<ParameterSaveData> HistorySaveData { get; set; }

        public ReinforcementDecisionMakerSaveData()
        {
            
        }

        public ReinforcementDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase, int historySize,
            float discountRatio, float[] lastRewards, string modelSaveDataJson, int inputDimention, float[] soulWeights,
            string optimizerType, int hiddenDimention, int[] subDecisionMakersKeys,
            IDecisionMakerSaveData[] subDecisionMakerValues,
            string[] keyOrder, float optimizerAlpha, List<ParameterSaveData> historySaveData)
        {
            OptimizerAlpha = optimizerAlpha;
            HistorySaveData = historySaveData;
            DecisionMakerBase = decisionMakerBase;
            HistorySize = historySize;
            DiscountRatio = discountRatio;
            LastRewards = lastRewards;
            ModelSaveDataJson = modelSaveDataJson;
            InputDimention = inputDimention;
            SoulWeights = soulWeights;
            OptimizerType = optimizerType;
            HiddenDimention = hiddenDimention;
            SubDecisionMakersKeys = subDecisionMakersKeys;
            SubDecisionMakerValues = subDecisionMakerValues;
            KeyOrder = keyOrder;
        }

        public IDecisionMaker Instantiate()
        {
            return new ReinforcementDecisionMaker(this);
        }
    }
}