using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MotionGenerator.Serialization;

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
        private int _manipulatableDimension;
        private List<IAction> _actions;

        public EvolutionarySequenceMaker(float epsilon, int minimumCandidates)
        {
            _epsilon = epsilon;
            _minimumCandidates = minimumCandidates;
            _lastAction = new GoForwardCoordinateAction("");
            _lastOutput = new Candidate(new List<MotionSequence>());
        }

        public EvolutionarySequenceMaker(EvolutionarySequenceMakerSaveData saveData)
            : base(saveData.SequenceMakerBase)
        {
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

        public new EvolutionarySequenceMakerSaveData Save()
        {
            return new EvolutionarySequenceMakerSaveData(
                base.Save(),
                _epsilon,
                _minimumCandidates,
                _lastAction.SaveAsInterface(),
                _lastOutput.Save(),
                _candidatesDict.ToDictionary(kv => kv.Key, kv => kv.Value.Select(x => x.Save()).ToList()),
                _randomMakerDict.ToDictionary(kv => kv.Key, kv => kv.Value.Save())
            );
        }

        public override ISequenceMakerSaveData SaveAsInterface()
        {
            return Save();
        }

        public override void Init(List<IAction> actions, List<int> manipulationDimensions)
        {
            _manipulatableDimension = manipulationDimensions.Sum();
            _actions = actions.Where(x => !(x is SubDecisionMakerAction)).ToList();
            _randomMakerDict = new Dictionary<string, RandomSequenceMaker>();
            _candidatesDict = new Dictionary<string, List<Candidate>>();
            foreach (var action in _actions)
            {
                var rsm = new RandomSequenceMaker(MaxSequenceLength, 1, 3);
                rsm.Init(new List<IAction> {action}, manipulationDimensions);
                _randomMakerDict.Add(action.Name, rsm);

                var candidates = new List<Candidate>(_minimumCandidates);
                for (var i = 0; i < candidates.Capacity; i++)
                {
                    candidates.Add(new Candidate(rsm.GenerateSequence(action)));
                }
                _candidatesDict.Add(action.Name, candidates);
            }
        }

        public override void Init(ISequenceMaker abstrctParent)
        {
            var parent = (EvolutionarySequenceMaker) abstrctParent;
            _actions = parent._actions;
            _epsilon = parent._epsilon;
            _minimumCandidates = parent._minimumCandidates;
            _candidatesDict = parent._candidatesDict.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(candidate => new Candidate(candidate)).ToList()
            );
            _randomMakerDict = parent._randomMakerDict.ToDictionary(
                kv => kv.Key,
                kv => new RandomSequenceMaker(kv.Value)
            );
        }

        public override void Restore(List<IAction> actions, List<int> manipulationDimensions)
        {
            _actions = actions;
            foreach (var kv in _randomMakerDict)
            {
                kv.Value.Restore(actions, manipulationDimensions);
            }
        }

        public override bool NeedToAlterManipulatables(List<int> manipulationDimensions)
        {
            return _manipulatableDimension != manipulationDimensions.Sum();
        }

        public override void AlterManipulatables(List<int> manipulationDimensions)
        {
            Init(_actions, manipulationDimensions);
        }

        public override List<MotionSequence> GenerateSequence(IAction action)
        {
//            foreach (var k in _candidatesDict.Keys)
//            {
//                Debug.Log(k.Name);
//            }
            var candidates = _candidatesDict[action.Name];
            _lastOutput = SelectByExpect(candidates);
            _lastAction = action;
            return _lastOutput.value;
        }

        private readonly ContinuousUniform _maintainRandom = new ContinuousUniform(0, 1.0, new MersenneTwister(0));

        void Maintain(IAction action)
        {
            var randomMaker = _randomMakerDict[action.Name];
            var candidates = _candidatesDict[action.Name];

            if (_maintainRandom.Sample() < 0.166667f * _epsilon)
            {
                candidates = candidates.OrderBy(o => o.mean).ToList(); // TODO performance check

                candidates.RemoveAt(0); // delete a worst candidate
                Candidate maxCandidate = candidates[candidates.Count - 1];

//				candidates.Add (new Candidate(randomMaker.GenerateSequence(action)));
//				candidates.Add (new Candidate (maxCandidate.value));
                candidates.Add(
                    new Candidate(randomMaker.GenerateSimilarSequence(action, maxCandidate.value, 0.3f * _epsilon, true)));

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