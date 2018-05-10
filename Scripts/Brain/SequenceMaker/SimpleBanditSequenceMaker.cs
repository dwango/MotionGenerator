using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class SimpleBanditSequenceMaker : SequenceMakerBase
    {
        private readonly float _epsilon; //epsilon-greedy
        private readonly int _minimumCandidates;
        private IAction _lastAction;
        private Candidate _lastOutput;
        private Dictionary<string, List<Candidate>> _candidatesDict;
        private Dictionary<string, RandomSequenceMaker> _randomMakerDict;
        private readonly int _numControlPoints;

        protected MersenneTwister RandomGenerator = new MersenneTwister(0);

        public SimpleBanditSequenceMaker(float epsilon, int minimumCandidates, int numControlPoints,
            int maxSequenceLength)
        {
            _epsilon = epsilon;
            _minimumCandidates = minimumCandidates;
            _numControlPoints = numControlPoints;
        }

        public SimpleBanditSequenceMaker(float epsilon, int minimumCandidates)
            : this(epsilon, minimumCandidates, 4, 100)
        {
        }

        public SimpleBanditSequenceMaker(SimpleBanditSequenceMakerSaveData saveData)
            : base(saveData.SequenceMakerBase)
        {
            _epsilon = saveData.Epsilon;
            _minimumCandidates = saveData.MinimumCandidates;
            _lastAction = saveData.LastAction.Instantiate();
            _lastOutput = new Candidate(saveData.LastOutput);
            _candidatesDict = saveData.CandidatesDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(x => new Candidate(x)).ToList()
            );
            _randomMakerDict = saveData.RandomMakerDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Instantiate()
            );
            _numControlPoints = saveData.NumControlPoints;
        }

        public new SimpleBanditSequenceMakerSaveData Save()
        {
            return new SimpleBanditSequenceMakerSaveData(
                base.Save(),
                _epsilon,
                _minimumCandidates,
                _lastAction.SaveAsInterface(),
                _lastOutput.Save(),
                _candidatesDict.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Select(x => x.Save()).ToList()
                ),
                _randomMakerDict.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.Save()
                ),
                _numControlPoints
            );
        }

        public override ISequenceMakerSaveData SaveAsInterface()
        {
            return Save();
        }

        public override void Init(List<IAction> actions, List<int> manipulationDimensions)
        {
            _randomMakerDict = new Dictionary<string, RandomSequenceMaker>();
            _candidatesDict = new Dictionary<string, List<Candidate>>();
            foreach (var action in actions)
            {
                var rsm = new RandomSequenceMaker(MaxSequenceLength, 1.0f, _numControlPoints);
                _randomMakerDict.Add(action.Name, rsm);
            }
        }

        public override void Restore(List<IAction> actions, List<int> manipulationDimensions)
        {
            _lastAction = actions.Find(x => x.Name == _lastAction.Name);
            foreach (var kv in _randomMakerDict)
            {
                var action = actions.Find(x => x.Name == kv.Key);
                kv.Value.Restore(new List<IAction> {action}, manipulationDimensions);
            }
        }

        public override List<MotionSequence> GenerateSequence(IAction action)
        {
            var candidates = _candidatesDict[action.Name];
            var epsilonUniform = new ContinuousUniform(0, 1.0, RandomGenerator);

            // epsilon-greedy algorithm
            if (epsilonUniform.Sample() > _epsilon)
            {
                _lastOutput = SelectByMax(candidates);
                Maintain(action, _lastOutput);
            }
            else
            {
                _lastOutput = SelectByCuriosity(candidates);
            }
            _lastAction = action;
            return _lastOutput.value;
        }

        private void ShowCandidates(IAction action)
        {
            var sb = new StringBuilder();

            var candidates = _candidatesDict[action.Name];
            sb.Append(action.Name + System.Environment.NewLine);

            foreach (var candidate in candidates)
            {
                var star = "";
                if (candidate.mean >= 0)
                {
                    star = new string('*', (int) Mathf.Floor(candidate.mean * 10));
                }
                sb.Append(star + candidate.ToString() + System.Environment.NewLine);
            }
            Debug.Log(sb.ToString());
        }

        private void Maintain(IAction action, Candidate maxCandidate)
        {
            var randomMaker = _randomMakerDict[action.Name];
            var candidates = _candidatesDict[action.Name];
            candidates.RemoveAll(candidate => maxCandidate.IsCompletelyBetterThan(candidate, 0.01f));
            candidates.RemoveAll(candidate => !maxCandidate.Equals(candidate) && maxCandidate.mean == candidate.mean);
            // avoid zero stickness
			foreach (var _ in Enumerable.Range(0, _minimumCandidates - candidates.Count))
            {
                candidates.Add(new Candidate(randomMaker.GenerateSequence(action)));
            }
            //ShowCandidates(action);
        }

        protected virtual Candidate SelectByMax(List<Candidate> candidates)
        {
            var maxCandidate = candidates[0];
            foreach (var candiate in candidates)
            {
                if (candiate.mean > maxCandidate.mean)
                {
                    maxCandidate = candiate;
                }
            }
            return maxCandidate;
        }

        protected virtual Candidate SelectByCuriosity(List<Candidate> candidates)
        {
            Candidate curiousCandidate = candidates[0];
            foreach (var candiate in candidates)
            {
                if (candiate.variance > curiousCandidate.variance)
                {
                    curiousCandidate = candiate;
                }
            }
            return curiousCandidate;
        }

        public override void Feedback(float reward, State lastState, State currentState)
        {
            if (_lastOutput == null) return;
            _lastOutput.Update(reward);
            Maintain(_lastAction, _lastOutput);
        }
    }
}