using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MotionGenerator.Serialization;
using System;

namespace MotionGenerator
{
    public class EvolutionarySequenceMaker : SequenceMakerBase
    {
        private float _epsilon; //epsilon-greedy
        private int _minimumCandidates;
        private IAction _lastAction;
        private Candidate _lastOutput;
        private Dictionary<string, List<Candidate>> _candidatesDict;
        private readonly ContinuousUniform _maintainRandom = new ContinuousUniform(0, 1.0, new MersenneTwister(0));
        private float _sequenceLength;

        public EvolutionarySequenceMaker(float epsilon, int minimumCandidates, float sequenceLength = -1f)
        {
            _epsilon = epsilon;
            _minimumCandidates = minimumCandidates;
            if (sequenceLength < 0)
            {
                _sequenceLength = MaxSequenceLength;
            }
            else
            {
                _sequenceLength = sequenceLength;
            }

            _lastAction = new GoForwardCoordinateAction("");
            _lastOutput = new Candidate(new Dictionary<Guid, MotionSequence>());
        }

        public EvolutionarySequenceMaker(EvolutionarySequenceMakerSaveData saveData)
        {
            _sequenceLength = MaxSequenceLength;
            _epsilon = saveData.Epsilon;
            _minimumCandidates = saveData.MinimumCandidates;
            _lastAction = saveData.LastAction.Instantiate();
            _lastOutput = new Candidate(saveData.LastOutput);
            _candidatesDict =
                saveData.CandidatesDict.ToDictionary(kv => kv.Key,
                    kv => kv.Value.Select(x => new Candidate(x)).ToList());
        }

        public EvolutionarySequenceMakerSaveData Save()
        {
            return new EvolutionarySequenceMakerSaveData(
                _epsilon,
                _minimumCandidates,
                _lastAction.SaveAsInterface(),
                _lastOutput.Save(),
                _candidatesDict.ToDictionary(kv => kv.Key, kv => kv.Value.Select(x => x.Save()).ToList())
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableDimensions)
        {
            base.Init(actions, manipulatableDimensions);
            var actionsList = actions.Where(x => !(x is SubDecisionMakerAction)).ToList();
            _candidatesDict = new Dictionary<string, List<Candidate>>();
            foreach (var action in actionsList)
            {
                var rsm = new RandomSequenceMaker(_sequenceLength, 1, 3, manipulatableDimensions);
                rsm.Init(new List<IAction> {action}, manipulatableDimensions);

                var candidates = new List<Candidate>(_minimumCandidates);
                for (var i = 0; i < candidates.Capacity; i++)
                {
                    candidates.Add(new Candidate(rsm.GenerateSequence(action)));
                }

                _candidatesDict.Add(action.Name, candidates);
            }
        }

        public override void Init(ISequenceMaker abstrctParent,
            Dictionary<Guid, int> manipulatableDimensions = null)
        {
            base.Init(abstrctParent, manipulatableDimensions);
            var parent = (EvolutionarySequenceMaker) abstrctParent;
            _epsilon = parent._epsilon;
            _minimumCandidates = parent._minimumCandidates;
            _candidatesDict = parent._candidatesDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(candidate => new Candidate(candidate, manipulatableDimensions)).ToList()
            );
        }

        public override Dictionary<Guid, MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            var candidates = _candidatesDict[action.Name];
            _lastOutput = SelectByExpect(candidates);
            _lastAction = action;
            return _lastOutput.value;
        }

        private void Maintain(IAction action)
        {
            if (_maintainRandom.Sample() < _epsilon)
            {
                var newCandidates = _candidatesDict[action.Name].OrderBy(o => o.mean).ToList();

                Candidate maxCandidate = newCandidates[newCandidates.Count - 1];
                // replace a worst candidate
                newCandidates[0] =
                    new Candidate(
                        RandomSequenceMaker.GenerateSimilarSequence(maxCandidate.value, _epsilon, timeRatio: 1f));

                _candidatesDict[action.Name] = newCandidates;
            }
        }

        private Candidate SelectByExpect(List<Candidate> candidates)
        {
            Candidate curiousCandidate = candidates[0];
            foreach (var candiate in candidates)
            {
                if (candiate.mean + (1.0f / (candiate.numTried + float.Epsilon)) >
                    curiousCandidate.mean + (1.0f / (curiousCandidate.numTried + float.Epsilon)))
                {
                    curiousCandidate = candiate;
                }
            }

            return curiousCandidate;
        }

        public override void Feedback(float reward, State lastState, State currentState)
        {
            if (currentState.ContainsKey(State.BasicKeys.AvoidSequenceMakerFeedback) &&
                currentState.GetAsFloat(State.BasicKeys.AvoidSequenceMakerFeedback) > 0.0001f)
            {
                return;
            }

            if (!(_lastOutput == null))
            {
//				Debug.Log (string.Format("lastAction:{0}, reward: {1}", lastAction.name, reward));
                _lastOutput.Update(reward);
                Maintain(_lastAction);
            }
        }
    }
}