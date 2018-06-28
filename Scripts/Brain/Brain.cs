using System;
using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class Brain : IBrain
    {
        private readonly IDecisionMaker _decisionMaker;
        private readonly ISequenceMaker _sequenceMaker;
        private IAction _currentAction;
        private readonly State _lastState = new State();

        private List<ISoul> _souls;

        public Brain(IDecisionMaker decisionMaker, ISequenceMaker sequenceMaker)
        {
            _decisionMaker = decisionMaker;
            _sequenceMaker = sequenceMaker;
        }

        public Brain(BrainSaveData saveData)
        {
            _decisionMaker = saveData.DecisionMaker.Instantiate();
            _sequenceMaker = saveData.SequenceMaker.Instantiate();
            _currentAction = saveData.CurrentAction.Instantiate();
            _lastState = new State(saveData.LastState);
        }

        public IBrainSaveData Save()
        {
            return new BrainSaveData(
                _decisionMaker.Save(),
                _sequenceMaker.SaveAsInterface(),
                _currentAction.SaveAsInterface(),
                _lastState.Save()
            );
        }

        public void Init(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul)
        {
            if (actions.Count == 0)
                throw new ArgumentException("need at least one action");
            _decisionMaker.Init(actions);
            _sequenceMaker.Init(actions, manipulatableDimensions);
            _souls = soul;
            _currentAction = actions[0];
        }

        public void Init(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul, IBrain iParentBrain)
        {
            var parentBrain = (Brain) iParentBrain;
            if (actions.Count == 0)
                throw new ArgumentException("need at least one action");
            _decisionMaker.Init(parentBrain._decisionMaker);
            _sequenceMaker.Init(parentBrain._sequenceMaker);
            _souls = soul;
            _currentAction = actions[0];
            
            if (_sequenceMaker.NeedToAlterManipulatables(manipulatableDimensions))
            {
                _sequenceMaker.AlterManipulatables(manipulatableDimensions);
            }
        }

        /// <summary>
        /// セーブデータからのロード時に、デシリアライズデータだけでは
        /// 足りない情報を復帰させる
        /// </summary>
        public void Restore(List<int> manipulatableDimensions, List<IAction> actions, List<ISoul> soul)
        {
            if (actions.Count == 0)
                throw new ArgumentException("need at least one action");
            _decisionMaker.Restore(actions);
            _sequenceMaker.Restore(actions, manipulatableDimensions);
            _souls = soul;
            _currentAction = actions.Find(x => x.Name == _currentAction.Name);
        }

        public void AlterSoulWeights(float[] soulWeights)
        {
            _decisionMaker.AlterSoulWeights(soulWeights);
        }

        public List<MotionSequence> GenerateMotionSequence(State state)
        {
            // feed back
            if (_lastState.Count != 0)
            {
                _sequenceMaker.Feedback(_currentAction.Reward(_lastState, state),
                    _lastState, state);
                _decisionMaker.Feedback(_souls.Select(x => x.Reward(_lastState, state)).ToList());
            }
            _lastState.ReplaceFrom(state);

            // forward
            _currentAction = _decisionMaker.DecideAction(state);
            return _sequenceMaker.GenerateSequence(_currentAction);
        }
        
        public void SetRandomActionProbability(float probability)
        {
            _decisionMaker.SetRandomActionProbability( probability);
        }

        public void ResetTrainer()
        {
            _decisionMaker.ResetModel();
        }
    }
}
