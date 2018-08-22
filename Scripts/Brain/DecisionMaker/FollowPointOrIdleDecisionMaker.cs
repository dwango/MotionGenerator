using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator
{
    public class FollowPointOrIdleDecisionMaker : DecisionMakerBase
    {
        private readonly List<string> _stateKeys = new List<string>();
        private readonly System.Random _random = new System.Random();
        private bool _isNegative;
        private float _stayableDistance;
        private float _nearbyDistance;

        private List<IAction> _locomotionActions;
        private List<IAction> _walkingLocomotionActions;
        private IAction _forwardAction;
        private IAction _stayAction;
        private IAction _restAction;
        private IAction _hopAction;
        private readonly List<IAction> _spinTurnActions = new List<IAction>();

        public FollowPointOrIdleDecisionMaker(string stateKeyPrimary, string stateKeySub = null,
            string stateKeySub2 = null, bool isNegative = false, float stayableDistance = 0f, float nearbyDistance = 0f)
        {
            _stateKeys.Add(stateKeyPrimary);
            if (stateKeySub != null)
            {
                _stateKeys.Add(stateKeySub);
            }

            if (stateKeySub2 != null)
            {
                _stateKeys.Add(stateKeySub2);
            }

            _isNegative = isNegative;
            _stayableDistance = stayableDistance;
            _nearbyDistance = nearbyDistance;
        }

        public FollowPointOrIdleDecisionMaker(FollowPointOrIdleDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
            _stateKeys = saveData.StateKeys;
            _isNegative = saveData.IsNegative;
            _stayableDistance = saveData.StayableDistance;
            _nearbyDistance = saveData.NearbyDistance;
        }

        public override IDecisionMakerSaveData Save()
        {
            return new FollowPointOrIdleDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save(),
                _stateKeys,
                _isNegative,
                _stayableDistance,
                _nearbyDistance
            );
        }

        private void Init()
        {
            _walkingLocomotionActions = Actions.Where(x => x.GetType() == typeof(WalkingLocomotionAction)).ToList();
            _locomotionActions = Actions.Where(x => x.GetType() == typeof(LocomotionAction)).ToList();
            Assert.IsTrue(_locomotionActions.Count % 4 == 0);

            for (var i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                var name = action.Name;
                if (action.GetType() == typeof(LocomotionAction))
                {
                    if (name == "forward")
                    {
                        _forwardAction = action;
                        if (_stayAction == null)
                        {
                            _stayAction = action;
                        }
                    }
                }
                else
                {
                    if (name == "stay")
                    {
                        _stayAction = action;
                    }
                    else if (name == "rest")
                    {
                        _restAction = action;
                    }
                    else if (name == "spinTurnLeft" || name == "spinTurnRight")
                    {
                        _spinTurnActions.Add(action);
                    }
                    else if (name == "hop")
                    {
                        _hopAction = action;
                    }
                }
            }

            if (_restAction == null)
            {
                _restAction = _stayAction;
            }

            if (_hopAction == null)
            {
                _hopAction = _stayAction;
            }
        }

        public override void Init(List<IAction> actions)
        {
            base.Init(actions);
            Init();
        }

        public override void Init(IDecisionMaker parent)
        {
            base.Init(parent);
            Init();
        }

        public override void Restore(List<IAction> actions)
        {
            base.Restore(actions);
            Init();
        }

        private IAction DecideIdleAction()
        {
            var rand = (float) _random.NextDouble();
            if (_spinTurnActions.Count > 0 && rand < 0.08f)
            {
                return _spinTurnActions[_random.Next(0, _spinTurnActions.Count)];
            }

            if (rand < 0.10f)
            {
                return _forwardAction;
            }

            if (rand < 0.20f)
            {
                return _hopAction;
            }

            return _restAction;
        }

        public override IAction DecideAction(State state)
        {
            var sightRange = state.GetAsFloat(State.BasicKeys.SightRange);
            var stayableRatio = _stayableDistance / sightRange;
            var nearbyRatio = _nearbyDistance / sightRange;

            for (var i = 0; i < _stateKeys.Count; i++)
            {
                var target = state.GetAsVector3(_stateKeys[i]);
                var magnitude = target.magnitude;
                var angle = Quaternion.LookRotation(target).eulerAngles.y + (_isNegative ? 180f : 0);

                if (magnitude <= 1f)
                {
                    // ターゲットから至近ならばstayする
                    if (magnitude <= stayableRatio)
                    {
                        return _stayAction;
                    }

                    // ターゲットに近くなってきたらSequenceMakerに歩きモーションを要求
                    if (magnitude <= nearbyRatio && _walkingLocomotionActions.Count > 0)
                    {
                        var direction = _walkingLocomotionActions.Count;
                        return _walkingLocomotionActions[((int) (angle * (direction * 2) / 360f) + 1) / 2 % direction];
                    }
                    else
                    {
                        var direction = _locomotionActions.Count;
                        return _locomotionActions[((int) (angle * (direction * 2) / 360f) + 1) / 2 % direction];
                    }
                }
            }

            return DecideIdleAction();
        }
    }
}