using System.Collections.Generic;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public interface ISequenceMaker
    {
        void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableDimensions);
        void Init(ISequenceMaker parent, Dictionary<Guid, int> manipulatableDimensions);
        void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId);
        Dictionary<Guid, MotionSequence> GenerateSequence(IAction action,
            State currentState = null);
        void Feedback(float reward, State lastState, State currentState);
        SequenceMakerSaveData SaveAsInterface();
    }
}