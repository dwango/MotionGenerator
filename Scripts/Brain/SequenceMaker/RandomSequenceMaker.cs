using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;
using System;
using UnityEngine.Assertions;

namespace MotionGenerator
{
    public class RandomSequenceMaker : SequenceMakerBase
    {
        private readonly float _timeRange;
        private readonly float _valueRange;
        private readonly int _numControlPoints;
        private Dictionary<Guid, int> _outputDimentions;

        private readonly MersenneTwister _randomGenerator;

        public RandomSequenceMaker(float timeRange, float valueRange, int numControlPoints,
            Dictionary<Guid, int> manipulatorDimensions)
        {
            _timeRange = timeRange;
            _valueRange = valueRange;
            _numControlPoints = numControlPoints;
            _randomGenerator = new MersenneTwister();
            _outputDimentions = manipulatorDimensions.ToDictionary(x => x.Key, x => x.Value);
        }

        public RandomSequenceMaker(RandomSequenceMaker other)
        {
            _timeRange = other._timeRange;
            _valueRange = other._valueRange;
            _numControlPoints = other._numControlPoints;
            _outputDimentions = other._outputDimentions.ToDictionary(x => x.Key, x => x.Value);
            _randomGenerator = other._randomGenerator;
        }

        public RandomSequenceMaker(RandomSequenceMaker other, Dictionary<Guid, int> newManipulatorDimensions)
        {
            _timeRange = other._timeRange;
            _valueRange = other._valueRange;
            _numControlPoints = other._numControlPoints;
            _outputDimentions = newManipulatorDimensions.ToDictionary(x => x.Key, x => x.Value);
            _randomGenerator = other._randomGenerator;
        }

        public RandomSequenceMaker(RandomSequenceMakerSaveData saveData)
        {
            _timeRange = saveData.TimeRange;
            _valueRange = saveData.ValueRange;
            _numControlPoints = saveData.NumControlPoints;
            _outputDimentions = saveData.OutputDimentions.ToDictionary(x => x.Key, x => x.Value);
            _randomGenerator = new MersenneTwister();
        }

        public RandomSequenceMakerSaveData Save()
        {
            return new RandomSequenceMakerSaveData(
                _timeRange,
                _valueRange,
                _numControlPoints,
                _outputDimentions.ToDictionary(x => x.Key, x => x.Value)
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override Dictionary<Guid, MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            var dict = new Dictionary<Guid, MotionSequence>();
            foreach (var item in _outputDimentions)
            {
                var dimension = item.Value;
                var manipulatableId = item.Key;
                dict.Add(manipulatableId, GenerateSynchronousSingleSequence(action, dimension));
            }

            return dict;
        }

        public MotionSequence GenerateSingleSequence(List<Guid> manipulatableIds, IAction action, int dimention)
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

            var sequence = new MotionTarget[InitialTimes.Length];
            for (var i = 0; i < InitialTimes.Length; i++)
            {
                var values = new float[dimention];
                values[0] = InitialForce;
                for (var d = 1; d < dimention; d++)
                {
                    // control point value
                    var radian = InitialTimes[i] * 2f * Mathf.PI + offset;
                    values[d] = 0.5f + ValueRangeRatio * Mathf.Sin(radian) / 2f; // [0,1]に正規化;
                }

                sequence[i] = new MotionTarget(InitialTimes[i] * _timeRange, values);
            }

            return new MotionSequence(sequence);
        }

        public static MotionSequence QuietMotionSequence(int dimension, float force, float timeRange)
        {
            const float InitialValue = 0.5f; // 値域が[0,1]なので中心の0.5

            var initialMotionTarget = new List<MotionTarget>();
            foreach (var time in InitialTimes)
            {
                var initialValues = Enumerable.Repeat(InitialValue, dimension).ToList();

                // forceは0次元目という決め打ちなので
                initialValues[0] = force;
                initialMotionTarget.Add(new MotionTarget(time * timeRange, initialValues));
            }

            return new MotionSequence(initialMotionTarget);
        }

        public Dictionary<Guid, MotionSequence> GenerateSimilarSequence(
            IAction action,
            Dictionary<Guid, MotionSequence> originalSequences,
            float noiseRate,
            bool enableNutralPosision
        )
        {
            var newMotionSequences = GenerateSequence(action); // TODO deep copy
            var perturbation = new Normal();
            var neutralPosition = enableNutralPosision ? 1 : 0; // last element is nutral position

            Assert.AreEqual(newMotionSequences.Count, originalSequences.Count);
            foreach (var manipulatableIndex in newMotionSequences.Keys)
            {
                var sequence = newMotionSequences[manipulatableIndex].Sequences;
                var original = originalSequences[manipulatableIndex].Sequences;
                Assert.AreEqual(sequence.Length, original.Length);
                for (int i = 0; i < sequence.Length - neutralPosition; i++)
                {
                    // EvolutionarySequenceMakerでControlPointのTimeに摂動を加える
                    var newSequence = new MotionTarget(original[i]);
                    newSequence.Time = original[i].Time +
                                       (float) perturbation.Sample() * noiseRate * 0.01f;
                    if (i + 1 < sequence.Length)
                    {
                        newSequence.Time = Mathf.Min(newSequence.Time, newSequence.Time);
                    }

                    if (i == 0)
                    {
                        newSequence.Time = Mathf.Max(newSequence.Time, 0);
                    }

                    for (int j = 0; j < newSequence.Values.Length; j++)
                    {
                        newSequence.Values[j] = original[i].Values[j] +
                                                (float) perturbation.Sample() * noiseRate;
                        newSequence.Values[j] = Mathf.Clamp(newSequence.Values[j], 0f, 1f);
                    }

                    sequence[i] = newSequence;
                }
            }

            return newMotionSequences;
        }

        public Dictionary<Guid, MotionSequence> ChangeTimeScale(
            IAction action,
            float ratio,
            Dictionary<Guid, MotionSequence> originalSequences
        )
        {
            var newMotionSequences = GenerateSequence(action); // TODO deep copy

            foreach (var manipulatableIndex in newMotionSequences.Keys)
            {
                var sequence = newMotionSequences[manipulatableIndex].Sequences;
                var original = originalSequences[manipulatableIndex].Sequences;
                for (int i = 0; i < sequence.Length; i++)
                {
                    var newSequence = new MotionTarget(original[i]);
                    newSequence.Time = original[i].Time * ratio;
                    sequence[i] = newSequence;
                }
            }

            return newMotionSequences;
        }

        public static Dictionary<Guid, MotionSequence> CopyValueWithSequenceMapping(
            Dictionary<Guid, MotionSequence> originalValue,
            Dictionary<Guid, int> newManipulatableDimensions)
        {
            const float InitialForceValue = 0.5f; //値域が[0,1]なので中心の0.5
            var motionDuration = originalValue.Values.Max(x => x.GetDuration());
            var timeRange = motionDuration;

            var manipulatableIds = newManipulatableDimensions.Keys;
            var value = new Dictionary<Guid, MotionSequence>();
            foreach (var manipulatableId in manipulatableIds)
            {
                MotionSequence motionSequence;
                if (!originalValue.ContainsKey(manipulatableId))
                {
                    motionSequence = RandomSequenceMaker.QuietMotionSequence(
                        newManipulatableDimensions[manipulatableId],
                        InitialForceValue, timeRange);
                }
                else
                {
                    // 親のManipulatorのSequenceをコピー
                    motionSequence = new MotionSequence(originalValue[manipulatableId]);
                }

                value.Add(manipulatableId, motionSequence);
            }

            return value;
        }
    }
}