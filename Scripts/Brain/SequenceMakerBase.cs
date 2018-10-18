using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public abstract class SequenceMakerBase : ISequenceMaker
    {
        protected readonly float MaxSequenceLength;
        private const float MaxSequenceLengthSec = 1.2f;
        private const float MinSequenceLengthSec = 0.4f;
        protected Dictionary<Guid, int> ManipulatableDimensions; 

        protected SequenceMakerBase()
        {
            var min = MinSequenceLengthSec;
            var max = MaxSequenceLengthSec;
            // Beta.Sample(2.0, 2.0) -> [0,1]
            MaxSequenceLength = min + (max - min) * (float) Beta.Sample(2.0, 2.0);
        }

        public abstract SequenceMakerSaveData SaveAsInterface();

        // new creature
        public virtual void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableDimensions)
        {
            ManipulatableDimensions = manipulatableDimensions;
        }

        // inherit 
        public virtual void Init(ISequenceMaker parent,
            Dictionary<Guid, int> manipulatableDimensions = null)
        {           
            ManipulatableDimensions = manipulatableDimensions;
        }

        // load
        public virtual void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId)
        {
            ManipulatableDimensions = manipulatableIdToSequenceId;
        }

        public abstract Dictionary<Guid, MotionSequence> GenerateSequence(IAction action, State currentState = null);

        public virtual void Feedback(float reward, State lastState, State currentState)
        {
        }
    }
}