using System;
using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public class WalkingLocomotionAction : ActionBase
    {
        public static List<IAction> EightDirections()
        {
            return new List<IAction>(LocomotionAction.EightDirectionsAsLocomotionActions()
                .Select(locomotionAction => new WalkingLocomotionAction(locomotionAction)).ToList());
        }

        public readonly LocomotionAction BaseAction;

        public WalkingLocomotionAction(LocomotionAction baseAction): base("Walking"+baseAction.Name)
        {
            BaseAction = baseAction;
        }
        
        private float HeuristicPenaltyWalking(State lastState, State nowState)
        {
            var penalty = 1f;
            var movement = LocomotionAction.GetLastMovement(lastState, nowState);
            var targetStepSize = nowState.GetAsFloat(State.BasicKeys.BodyScale);

            var overDistance = movement.magnitude - targetStepSize;
            if (0f < overDistance)
            {
                // オーバーランはペナルティ
                var overPenalty = targetStepSize - overDistance;
                if (overPenalty > 0f)
                {
                    penalty *= overPenalty;
                }
                else
                {
                    // マイナスだと逆方向になるので0
                    penalty = 0f;
                }
            }

            return penalty;
        }

        public override float Reward(State lastState, State nowState)
        {
            if (!(nowState.ContainsKey(State.BasicKeys.BodyScale)))
            {
                throw new ArgumentException($"need {State.BasicKeys.BodyScale}");
            }

            return BaseAction.Reward(lastState, nowState) * HeuristicPenaltyWalking(lastState, nowState);
        }

        public override ActionSaveData SaveAsInterface()
        {
            return new ActionSaveData(GetType(),
                MotionGeneratorSerialization.Serialize(BaseAction.SaveAsLocomotion()));
        }

        public override IAction Clone()
        {
            return new WalkingLocomotionAction(BaseAction.Clone() as LocomotionAction);
        }
    }
}