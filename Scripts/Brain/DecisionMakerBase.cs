using System;
using System.Collections.Generic;
using System.Linq;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public abstract class DecisionMakerBase : IDecisionMaker
    {
        public List<IAction> Actions { get; protected set; }

        protected DecisionMakerBase()
        {
            Actions = new List<IAction>();
        }

        protected DecisionMakerBase(DecisionMakerBaseSaveData saveData)
        {
            Actions = saveData.Actions.Select(x => x.Instantiate()).ToList();
        }

        public virtual void AlterSoulWeights(float[] soulWeights)
        {
            // soulを必要としないDMには何も効果がない
        }

        public virtual void SetRandomActionProbability(float probability)
        {
        }

        public virtual void ResetModel()
        {
        }
        
        public virtual IDecisionMakerSaveData Save()
        {
            return new DecisionMakerBaseSaveData(
                Actions.Select(x => x.SaveAsInterface()).ToList()
            );
        }

        // new
        public virtual void Init(List<IAction> actions)
        {
            if (actions.Count == 0)
            {
                throw new ArgumentException("need at least one action");
            }
            Actions = actions;
        }

        // inherit
        public virtual void Init(IDecisionMaker parent)
        {
            Actions = ((DecisionMakerBase) parent).Actions;
        }

        // load
        public virtual void Restore(List<IAction> actions)
        {
            Actions = Actions.Select(x => actions.Find(y => y.Name == x.Name)).ToList();
        }

        public abstract IAction DecideAction(State state);

        public virtual void Feedback(List<float> reward)
        {
        }
       
    }
}