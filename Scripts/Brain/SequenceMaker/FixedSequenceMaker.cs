using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public class FixedSequenceMaker : SequenceMakerBase
    {
        private Dictionary<string, Dictionary<Guid, MotionSequence>> _motionDict;

        public FixedSequenceMaker(Dictionary<string, Dictionary<Guid, MotionSequence>> motionDict)
        {
            _motionDict = motionDict;
        }

        public FixedSequenceMaker(FixedSequenceMakerSaveData saveData)
        {
            _motionDict = saveData.MotionDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToDictionary(x=>x.Key , x => new MotionSequence(x.Value))
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(
                new FixedSequenceMakerSaveData(
                    _motionDict.ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value.ToDictionary(x=>x.Key , x => x.Value.Save())
                    )
                )));
        }

        public override void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId)
        {
            base.Restore(actions, manipulatableIdToSequenceId);
            _motionDict = _motionDict.ToDictionary(
                kv => actions.Find(x => x.Name == kv.Key).Name,
                kv => kv.Value);
        }

        public override Dictionary<Guid, MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            return _motionDict[action.Name];
        }
    }
}