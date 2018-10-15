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
        private Dictionary<string, RandomSequenceMaker> _randomMakerDict;
        private List<IAction> _actions;
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
            _lastOutput = new Candidate(new List<MotionSequence>());
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
            _randomMakerDict =
                saveData.RandomMakerDict.ToDictionary(kv => kv.Key, kv => new RandomSequenceMaker(kv.Value));
        }

        public EvolutionarySequenceMakerSaveData Save()
        {
            return new EvolutionarySequenceMakerSaveData(
                _epsilon,
                _minimumCandidates,
                _lastAction.SaveAsInterface(),
                _lastOutput.Save(),
                _candidatesDict.ToDictionary(kv => kv.Key, kv => kv.Value.Select(x => x.Save()).ToList()),
                _randomMakerDict.ToDictionary(kv => kv.Key, kv => kv.Value.Save())
            );
        }

        public override SequenceMakerSaveData SaveAsInterface()
        {
            return new SequenceMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override void Init(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId, List<int> manipulationDimensions)
        {
            base.Init(actions, manipulatableIdToSequenceId, manipulationDimensions);
            _actions = actions.Where(x => !(x is SubDecisionMakerAction)).ToList();
            _randomMakerDict = new Dictionary<string, RandomSequenceMaker>();
            _candidatesDict = new Dictionary<string, List<Candidate>>();
            foreach (var action in _actions)
            {
                var rsm = new RandomSequenceMaker(_sequenceLength, 1, 3, manipulationDimensions);
                rsm.Init(new List<IAction> {action}, manipulatableIdToSequenceId, manipulationDimensions);
                _randomMakerDict.Add(action.Name, rsm);

                var candidates = new List<Candidate>(_minimumCandidates);
                for (var i = 0; i < candidates.Capacity; i++)
                {
                    candidates.Add(new Candidate(rsm.GenerateSequence(action)));
                }

                _candidatesDict.Add(action.Name, candidates);
            }
        }

        public override void Init(ISequenceMaker abstrctParent, Dictionary<Guid, int> manipulatableIdToSequenceId,
            List<int> manipulationDimensions)
        {
            base.Init(abstrctParent, manipulatableIdToSequenceId, manipulationDimensions);
            var parent = (EvolutionarySequenceMaker) abstrctParent;
            _actions = parent._actions;
            _epsilon = parent._epsilon;
            _minimumCandidates = parent._minimumCandidates;
            _randomMakerDict = parent._randomMakerDict.ToDictionary(
                kv => kv.Key,
                kv => new RandomSequenceMaker(kv.Value, manipulationDimensions)
            );
            var childSequenceIdToParentSequenceId = GenerateChildSequenceIdToParentSequenceIdMapping(
                manipulatableIdToSequenceId, parent.ManipulatableIdToSequenceId); 
            _candidatesDict = parent._candidatesDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(candidate => new Candidate(candidate, manipulationDimensions, childSequenceIdToParentSequenceId)).ToList()
            );
        }
        
        public override void Restore(List<IAction> actions, Dictionary<Guid, int> manipulatableIdToSequenceId)
        {
            base.Restore(actions, manipulatableIdToSequenceId);
            _actions = actions;
            foreach (var kv in _randomMakerDict)
            {
                kv.Value.Restore(actions, manipulatableIdToSequenceId);
            }
        }
        
        public override List<MotionSequence> GenerateSequence(IAction action, State currentState = null)
        {
            var candidates = _candidatesDict[action.Name];
            _lastOutput = SelectByExpect(candidates);
            _lastAction = action;
            return _lastOutput.value;
        }

        void Maintain(IAction action)
        {
            var randomMaker = _randomMakerDict[action.Name];
            var candidates = _candidatesDict[action.Name];

            if (_maintainRandom.Sample() < _epsilon)
            {
                candidates = candidates.OrderBy(o => o.mean).ToList();

                candidates.RemoveAt(0); // delete a worst candidate
                Candidate maxCandidate = candidates[candidates.Count - 1];
                candidates.Add(
                    new Candidate(randomMaker.GenerateSimilarSequence(action, maxCandidate.value, _epsilon, true)));

                _candidatesDict[action.Name] = candidates;
            }
        }

        Candidate SelectByExpect(List<Candidate> candidates)
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
            if (!(_lastOutput == null))
            {
//				Debug.Log (string.Format("lastAction:{0}, reward: {1}", lastAction.name, reward));
                _lastOutput.Update(reward);
                Maintain(_lastAction);
            }
        }
    }
}