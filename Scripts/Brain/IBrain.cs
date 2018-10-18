using System.Collections.Generic;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public interface IBrain
    {
        void Init(Dictionary<Guid, int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul);
        void Init(Dictionary<Guid, int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul, IBrain parent);
        void Restore(Dictionary<Guid, int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul);
        void AlterSoulWeights(float[] soulWeights);
        void SetRandomActionProbability(float probability);
        void ResetModel();
        Dictionary<Guid, MotionSequence> GenerateMotionSequence(State state);
        IBrainSaveData Save();
    }
}