using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator
{
    public class FollowPointDecisionMaker : DecisionMakerBase
    {
        private readonly List<string> _stateKeys = new List<string>();
        private readonly System.Random _random = new System.Random();
        private bool _isNegative;

        private List<IAction> _locomotionActions;

        public FollowPointDecisionMaker(string stateKeyPrimary, string stateKeySub = null, string stateKeySub2 = null,
            bool isNegative = false)
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
        }

        public FollowPointDecisionMaker(FollowPointDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
            _stateKeys = saveData.StateKeys;
            _isNegative = saveData.IsNegative;
        }

        public override IDecisionMakerSaveData Save()
        {
            return new FollowPointDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save(),
                _stateKeys, _isNegative
            );
        }


        private void Init()
        {
            _locomotionActions = Actions.Where(x => x.GetType() == typeof(LocomotionAction)).ToList();
            Assert.IsTrue(_locomotionActions.Count % 4 == 0);
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
        
        public override IAction DecideAction(State state)
        {
            var direction = _locomotionActions.Count;

            for (var i = 0; i < _stateKeys.Count; i++)
            {
                var stateKey = _stateKeys[i];

                int targetDir;
                if (!state.ContainsKey(stateKey) ||
                    state[stateKey][0] == 1f && state[stateKey][1] == 1f && state[stateKey][2] == 1f)
                {
                    // no food
                    if (i < _stateKeys.Count - 1)
                        continue;

                    targetDir = _random.Next(direction);
                }
                else
                {
                    // rule based following action
                    var target = state.GetAsVector3(stateKey);

                    // CentralBodyのひっくり返りを無視する場合
                    // （ひっくり返ったら、暫くは今までのモーションを試すが、あまり上手く進めない場合は、ひっくり返った状態で動けるモーションを学習し始める）
                    var angle = Quaternion.LookRotation(target).eulerAngles.y + (_isNegative ? 180f : 0);
                    targetDir = ((int) (angle * (direction * 2) / 360f) + 1) / 2 % direction;
                }

                return _locomotionActions[targetDir];
            }

            return null; // bug!
        }
    }
}