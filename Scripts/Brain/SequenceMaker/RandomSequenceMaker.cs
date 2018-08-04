using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public class RandomSequenceMaker : SequenceMakerBase
    {
        private readonly float _timeRange;
        private readonly float _valueRange;
        private readonly int _numControlPoints;
        private List<int> _outputDimentions;

        private readonly MersenneTwister _randomGenerator;
        private const string ThisTypeString = "RandomSequenceMaker";

        static RandomSequenceMaker()
        {
            SequenceMakerSaveData.AddDeserializer(ThisTypeString, baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<LocomotionSequenceMakerSaveData>(baseData);
                return new LocomotionSequenceMaker(saveData);
            });
        }

        public RandomSequenceMaker(float timeRange, float valueRange, int numControlPoints)
        {
            _timeRange = timeRange;
            _valueRange = valueRange;
            _numControlPoints = numControlPoints;
            _randomGenerator = new MersenneTwister();
        }

        public RandomSequenceMaker(RandomSequenceMaker other)
        {
            _timeRange = other._timeRange;
            _valueRange = other._valueRange;
            _numControlPoints = other._numControlPoints;
            _outputDimentions = other._outputDimentions.ToList();
            _randomGenerator = other._randomGenerator;
        }

        public RandomSequenceMaker(RandomSequenceMakerSaveData saveData)
        {
            _timeRange = saveData.TimeRange;
            _valueRange = saveData.ValueRange;
            _numControlPoints = saveData.NumControlPoints;
            _outputDimentions = saveData.OutputDimentions;
            _randomGenerator = new MersenneTwister();
        }

        public RandomSequenceMakerSaveData Save()
        {
            return new RandomSequenceMakerSaveData(
                _timeRange,
                _valueRange,
                _numControlPoints,
                _outputDimentions
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(ThisTypeString, MotionGeneratorSerialization.Serialize(Save()));
        }
        
        public override void Init(List<IAction> actions, List<int> manipulationDimensions)
        {
            _outputDimentions = manipulationDimensions;
        }

        public override void Restore(List<IAction> actions, List<int> manipulatableDimensions)
        {
            this.Init(actions, manipulatableDimensions);
        }

        public override List<MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            return _outputDimentions.Select(x => GenerateSynchronousSingleSequence(action, x)).ToList();
        }

        public MotionSequence GenerateSingleSequence(IAction action, int dimention)
        {
            var timeUniform = new ContinuousUniform(0, _timeRange, _randomGenerator);
            var times = Enumerable.Range(0, _numControlPoints)
                .Select(v => (float) timeUniform.Sample())
                .OrderBy(v => v)
                .ToList();

            var valueUniform = new ContinuousUniform(0, _valueRange, _randomGenerator);
            var sequence = times
                .Select(time => new MotionTarget(
                    time,
                    Enumerable.Range(0, dimention).Select(v => (float) valueUniform.Sample()).ToList())
                )
                .ToList();

            // to neutral
            sequence.Add(new MotionTarget(_timeRange, Enumerable.Range(0, dimention).Select(v => 0.5f).ToList()));
            return new MotionSequence(sequence);
        }

        // Sin関数を３つのコントロールポイントで近似しようとすると、最大値、最小値、モーション切替時の中間の値の３つとなり、以下の値となる
        private static readonly float[] InitialTimes = {0.25f, 0.75f, 1f};

        public MotionSequence GenerateSynchronousSingleSequence(IAction action, int dimention)
        {
            var uniform = new ContinuousUniform(0, 1, _randomGenerator);
            var ValueRangeRatio = (float) uniform.Sample();
            var InitialForce = (float) uniform.Sample();

            if (_numControlPoints != InitialTimes.Length)
            {
                throw new ArgumentException("コントロールポイント数が不一致");
            }

            // 位相にノイズを入れる
            var offset = Mathf.PI * (uniform.Sample() > 0.5 ? 1 : 0);

            var sequence = new List<MotionTarget>();
            foreach (var time in InitialTimes)
            {
                var values = new List<float> {InitialForce};
                for (var d = values.Count; d < dimention; d++)
                {
                    // control point value
                    var radian = time * 2f * Mathf.PI + offset;
                    var value = 0.5f + ValueRangeRatio * Mathf.Sin(radian) / 2f; // [0,1]に正規化
                    values.Add(value);
                }
                sequence.Add(new MotionTarget(time * _timeRange, values));
            }
            return new MotionSequence(sequence);
        }

        public List<MotionSequence> GenerateSimilarSequence(
            IAction action,
            List<MotionSequence> originalSequences,
            float noiseRate,
            bool enableNutralPosision
        )
        {
            var newMotionSequences = GenerateSequence(action); // TODO deep copy
            var perturbation = new Normal();
            var neutralPosition = enableNutralPosision ? 1 : 0; // last element is nutral position

            for (int manipulatableIndex = 0; manipulatableIndex < newMotionSequences.Count; manipulatableIndex++)
            {
                var sequence = newMotionSequences[manipulatableIndex].Sequence;
                var original = originalSequences[manipulatableIndex].Sequence;
                UnityEngine.Assertions.Assert.AreEqual(sequence.Count, original.Count);
                for (int i = 0; i < sequence.Count - neutralPosition; i++)
                {
                    // EvolutionarySequenceMakerでControlPointのTimeに摂動を加える
                    sequence[i].time = original[i].time +
                                       (float) perturbation.Sample() * noiseRate * 0.01f;
                    if (i + 1 < sequence.Count)
                    {
                        sequence[i].time = Mathf.Min(sequence[i].time, sequence[i + 1].time);
                    }
                    if (i == 0)
                    {
                        sequence[i].time = Mathf.Max(sequence[i].time, 0);
                    }

                    for (int j = 0; j < sequence[i].value.Count; j++)
                    {
                        sequence[i].value[j] = original[i].value[j] +
                                               (float) perturbation.Sample() * noiseRate;
                        sequence[i].value[j] = Mathf.Clamp(sequence[i].value[j], 0f, 1f);
                    }
                }
            }
            return newMotionSequences;
        }

        public List<MotionSequence> ChangeTimeScale(
            IAction action,
            float ratio,
            List<MotionSequence> originalSequences
        )
        {
            var newMotionSequences = GenerateSequence(action); // TODO deep copy

            for (int manipulatableIndex = 0; manipulatableIndex < newMotionSequences.Count; manipulatableIndex++)
            {
                var sequence = newMotionSequences[manipulatableIndex].Sequence;
                var original = originalSequences[manipulatableIndex].Sequence;
                for (int i = 0; i < sequence.Count; i++)
                {
                    sequence[i].time = original[i].time * ratio;
                    for (int j = 0; j < sequence[i].value.Count; j++)
                    {
                        sequence[i].value[j] = original[i].value[j];
                    }
                }
            }
            return newMotionSequences;
        }
    }
}