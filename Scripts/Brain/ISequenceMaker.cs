using System.Collections.Generic;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public interface ISequenceMaker
    {
        void Init(List<IAction> actions, List<int> manipulationDimensions);
        void Init(ISequenceMaker parent);
        void AlterManipulatables(List<int> manipulationDimensions);
        bool NeedToAlterManipulatables(List<int> manipulationDimensions);
        void Restore(List<IAction> actions, List<int> manipulatableDimensions);
        List<MotionSequence> GenerateSequence(IAction action, State currentState);
        void Feedback(float reward, State lastState, State currentState);
        SequenceMakerSaveData SaveAsInterface();
    }
}