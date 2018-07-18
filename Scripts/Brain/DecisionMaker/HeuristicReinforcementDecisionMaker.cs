using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class HeuristicReinforcementDecisionMaker : ReinforcementDecisionMaker
    {
        private int _practiceDecisionMakerIndex;
        private int _practiceDecisionCount;
        private int _practiceDecisionInitialCount;

        private int _emergencyDecisionMakerIndex;
        private float _emergencyEnergyRatio;

        public HeuristicReinforcementDecisionMaker(int historySize = 100000, float discountRatio = 0.9f,
            float[] soulWeights = null, string optimizerType = "adam", int hiddenDimention = 32,
            int practiceDecisionMakerIndex = -1, int practiceDecisionCount = 256,
            int emergencyDecisionMakerIndex = -1, float emergencyEnergyRatio = 0.1f,
            bool enableRandomForgetting = false)
            : base(historySize, discountRatio, soulWeights, optimizerType, hiddenDimention, 
                enableRandomForgetting : enableRandomForgetting)
        {
            _practiceDecisionMakerIndex = practiceDecisionMakerIndex;
            _practiceDecisionCount = _practiceDecisionInitialCount = practiceDecisionCount;
            _emergencyDecisionMakerIndex = emergencyDecisionMakerIndex;
            _emergencyEnergyRatio = emergencyEnergyRatio;
        }


        public HeuristicReinforcementDecisionMaker(HeuristicReinforcementDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
            _practiceDecisionMakerIndex = saveData.PracticeDecisionMakerIndex;
            _practiceDecisionCount = saveData.PracticeDecisionCount;
            _practiceDecisionInitialCount = saveData.PracticeDecisionInitialCount;
            _emergencyDecisionMakerIndex = saveData.EmergencyDecisionMakerIndex;
            _emergencyEnergyRatio = saveData.EmergencyEnergyRatio;
        }

        public override IDecisionMakerSaveData Save()
        {
            return new HeuristicReinforcementDecisionMakerSaveData((ReinforcementDecisionMakerSaveData) base.Save(),
                _practiceDecisionMakerIndex,
                _practiceDecisionCount,
                _practiceDecisionInitialCount,
                _emergencyDecisionMakerIndex,
                _emergencyEnergyRatio
            );
        }


        public override void Init(List<IAction> actions)
        {
            base.Init(actions);
            InitPracticeMotion(actions);
            InitEmergencyMotion(actions);
        }

        public override IAction DecideAction(State state)
        {
            // ルール1: モーション試行のため、初めて生まれてから一定回数は強制行動するルール
            var action = PracticeMotion(state);
            if (action != null)
            {
                return action;
            }

            // ルール2: エネルギーが一定以下だと強制行動するルール
            action = EmergencyMotion(state);
            if (action != null)
            {
                return action;
            }

            // その他、本能ルールをここに追記していく
            // TODO: 3つになったらファンクタ化

            // なにもないなら、ReinforcementDecisionMakerにDecisionさせる
            return base.DecideAction(state);
        }

        public override void ResetModel()
        {
            _practiceDecisionCount = _practiceDecisionInitialCount; // カウンタを最初に戻す
            base.ResetModel();
        }
        
        private void InitPracticeMotion(List<IAction> actions)
        {
            if (_practiceDecisionMakerIndex < 0)
            {
                // 最初の一つ目がFollowPointDMだと想定
                _practiceDecisionMakerIndex = SubDecisionMakers.First().Key;
            }
        }

        private IAction PracticeMotion(State state)
        {
            if (_practiceDecisionCount <= 0)
            {
                return null;
            }

            var action = ForceAction(state, _practiceDecisionMakerIndex);

            if (action != null && action is LocomotionAction)
            {
                _practiceDecisionCount--;
            }

            return action;
        }
        
        private void InitEmergencyMotion(List<IAction> actions)
        {
            if (_emergencyDecisionMakerIndex < 0)
            {
                // 最初の一つ目がFollowPointDMだと想定
                _emergencyDecisionMakerIndex = SubDecisionMakers.First().Key;
            }
        }

        private IAction EmergencyMotion(State state)
        {
            if (state[State.BasicKeys.OrganEnergy][0] > _emergencyEnergyRatio)
            {
                return null;
            }

            return ForceAction(state, _emergencyDecisionMakerIndex);
        }
    }
}