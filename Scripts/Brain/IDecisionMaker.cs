using System.Collections.Generic;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public interface IDecisionMaker
    {
        void Init(List<IAction> actions);
        void Init(IDecisionMaker parent);
        void Restore(List<IAction> actions);
        IAction DecideAction(State state);
        void Feedback(List<float> reward);
        void AlterSoulWeights(float[] soulWeights);
        void SetRandomActionProbability(float probability);
        void ResetModel();
        IDecisionMakerSaveData Save();
    }
}