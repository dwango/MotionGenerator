﻿using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class FixedSequenceMaker : SequenceMakerBase
    {
        private Dictionary<string, List<MotionSequence>> _motionDict;

        public FixedSequenceMaker(Dictionary<string, List<MotionSequence>> motionDict)
        {
            _motionDict = motionDict;
        }

        public FixedSequenceMaker(FixedSequenceMakerSaveData saveData)
            : base(saveData.SequenceMakerBase)
        {
            _motionDict = saveData.MotionDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(x => new MotionSequence(x)).ToList()
            );
        }

        public override ISequenceMakerSaveData SaveAsInterface()
        {
            return new FixedSequenceMakerSaveData(
                base.Save(),
                _motionDict.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Select(x => x.Save()).ToList()
                )
            );
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

        public override List<MotionSequence> GenerateSequence(IAction action)
        {
            return _motionDict[action.Name];
        }
    }
}