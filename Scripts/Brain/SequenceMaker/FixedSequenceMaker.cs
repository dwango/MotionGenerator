﻿using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class FixedSequenceMaker : SequenceMakerBase
    {
        private Dictionary<string, List<MotionSequence>> _motionDict;

        private const string ThisTypeString = "FixedSequenceMaker";

        static FixedSequenceMaker()
        {
            SequenceMakerSaveData.AddDeserializer(ThisTypeString, baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<FixedSequenceMakerSaveData>(baseData);
                return new FixedSequenceMaker(saveData);
            });
        }

        public FixedSequenceMaker(Dictionary<string, List<MotionSequence>> motionDict)
        {
            _motionDict = motionDict;
        }

        public FixedSequenceMaker(FixedSequenceMakerSaveData saveData)
        {
            _motionDict = saveData.MotionDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(x => new MotionSequence(x)).ToList()
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(ThisTypeString, MotionGeneratorSerialization.Serialize(
                new FixedSequenceMakerSaveData(
                    _motionDict.ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value.Select(x => x.Save()).ToList()
                    )
                )));
        }

        public override void Init(List<IAction> actions, List<int> manipulationDimensions)
        {
        }

        public override void Restore(List<IAction> actions, List<int> manipulationDimensions)
        {
            _motionDict = _motionDict.ToDictionary(
                kv => actions.Find(x => x.Name == kv.Key).Name,
                kv => kv.Value);
        }

        public override List<MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            return _motionDict[action.Name];
        }
    }
}