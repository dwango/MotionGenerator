using System.Collections.Generic;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public interface IBrain
    {
        void Init(List<int> manipulatableDimensions, Dictionary<Guid, int> manipulatableIdToSequenceId, List<IAction> actions, List<ISoul> soul);
        void Init(List<int> manipulatableDimensions, Dictionary<Guid, int> manipulatableIdToSequenceId, List<IAction> actions, List<ISoul> soul, IBrain parent);
        void Restore(List<int> manipulatableDimensions,
            Dictionary<Guid, int> manipulatableIdToSequenceId, List<IAction> actions, List<ISoul> soul);
        void AlterSoulWeights(float[] soulWeights);
        void SetRandomActionProbability(float probability);
        void ResetModel();
        List<MotionSequence> GenerateMotionSequence(State state);
        IBrainSaveData Save();
    }
}