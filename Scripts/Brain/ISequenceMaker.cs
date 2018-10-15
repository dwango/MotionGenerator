using System.Collections.Generic;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public interface ISequenceMaker
    {
        void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId, List<int> manipulationDimensions);
        void Init(ISequenceMaker parent, Dictionary<Guid, int> manipulatableIdToSequenceId, List<int> manipulationDimensions=null);
        void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId);
        List<MotionSequence> GenerateSequence(IAction action, State currentState);
        void Feedback(float reward, State lastState, State currentState);
        SequenceMakerSaveData SaveAsInterface();
    }
}