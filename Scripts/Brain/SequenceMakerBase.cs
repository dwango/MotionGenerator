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
        protected Dictionary<Guid, int> ManipulatableIdToSequenceId; 

        protected SequenceMakerBase()
        {
            var min = MinSequenceLengthSec;
            var max = MaxSequenceLengthSec;
            // Beta.Sample(2.0, 2.0) -> [0,1]
            MaxSequenceLength = min + (max - min) * (float) Beta.Sample(2.0, 2.0);
        }

        public abstract SequenceMakerSaveData SaveAsInterface();

        // new creature
        public virtual void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId, List<int> manipulationDimensions)
        {
            ManipulatableIdToSequenceId = manipulatableIdToSequenceId;
        }

        // inherit 
        public virtual void Init(ISequenceMaker parent, Dictionary<Guid, int> manipulatableIdToSequenceId,
            List<int> manipulationDimensions)
        {           
            ManipulatableIdToSequenceId = manipulatableIdToSequenceId;
        }

        // load
        public virtual void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId)
        {
            ManipulatableIdToSequenceId = manipulatableIdToSequenceId;
        }

        public abstract List<MotionSequence> GenerateSequence(IAction action, State currentState = null);

        public virtual void Feedback(float reward, State lastState, State currentState)
        {
        }

        protected static Dictionary<int, int> GenerateChildSequenceIdToParentSequenceIdMapping(
            Dictionary<Guid, int> childManipulatableIdToSequenceId,
            Dictionary<Guid, int> parentManipulatableIdToSequenceId)
        {
            var childSequenceIdToParentSequenceId = new Dictionary<int, int>();
            foreach (var manipulatableId in childManipulatableIdToSequenceId.Keys)
            {
                const int referenceSequenceIdNotExists = -1;
                var childSequenceId = childManipulatableIdToSequenceId[manipulatableId];
                var parentSequenceId = parentManipulatableIdToSequenceId.ContainsKey(manipulatableId)
                    ? parentManipulatableIdToSequenceId[manipulatableId]
                    : referenceSequenceIdNotExists;

                childSequenceIdToParentSequenceId.Add(
                    childSequenceId,
                    parentSequenceId);
            }

            return childSequenceIdToParentSequenceId;
        }
    }
}