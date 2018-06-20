using System.Collections.Generic;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public interface IBrain
    {
        void Init(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul);
        void Init(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul, IBrain parent);
        void Restore(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul);
        void AlterSoulWeights(float[] soulWeights);
        void SetRandomActionProbability(float probability);
        void ResetTrainer();
        List<MotionSequence> GenerateMotionSequence(State state);
        IBrainSaveData Save();
    }
}