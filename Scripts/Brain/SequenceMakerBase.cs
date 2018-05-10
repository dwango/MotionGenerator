using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public abstract class SequenceMakerBase : ISequenceMaker
    {
        protected readonly int MaxSequenceLength;
        private const float MaxSequenceLengthSec = 1.2f;
        private const float MinSequenceLengthSec = 0.4f;

        protected SequenceMakerBase()
        {
            var min = MinSequenceLengthSec / Time.fixedDeltaTime;
            var max = MaxSequenceLengthSec / Time.fixedDeltaTime;
            // Beta.Sample(2.0, 2.0) -> [0,1]
            MaxSequenceLength = (int) Math.Ceiling(min + (max - min) * (float) Beta.Sample(2.0, 2.0));
        }

        protected SequenceMakerBase(SequenceMakerBaseSaveData saveData)
        {
        }

        public SequenceMakerBaseSaveData Save()
        {
            return new SequenceMakerBaseSaveData();
        }

        public virtual ISequenceMakerSaveData SaveAsInterface()
        {
            return Save();
        }

        // new
        public abstract void Init(List<IAction> actions, List<int> manipulationDimensions);

        // inherit
        public virtual void Init(ISequenceMaker parent)
        {
            throw new NotImplementedException();
        }

        // load
        public abstract void Restore(List<IAction> actions, List<int> manipulatableDimensions);

        public virtual void AlterManipulatables(List<int> manipulationDimensions) 
        {
        }

        public virtual bool NeedToAlterManipulatables(List<int> manipulationDimensions)
        {
            return false;
        }

        public abstract List<MotionSequence> GenerateSequence(IAction action);

        public virtual void Feedback(float reward, State lastState, State currentState)
        {
        }
    }
}