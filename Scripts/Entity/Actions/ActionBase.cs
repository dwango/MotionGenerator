using System.Collections.Generic;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public abstract class ActionBase : IAction
    {
        public string Name { get; private set; }

        protected ActionBase() : this("")
        {
        }

        protected ActionBase(string name)
        {
            Name = name;
        }

        protected ActionBase(ActionBase src)
        {
            Name = src.Name;
        }

        protected ActionBase(ActionBaseSaveData saveData)
        {
            Name = saveData.Name;
        }

        public ActionBaseSaveData Save()
        {
            return new ActionBaseSaveData(
                Name
            );
        }

        public virtual IActionSaveData SaveAsInterface()
        {
            return Save();
        }

        public abstract IAction Clone();
        public abstract float Reward(State lastState, State nowState);
    }
}
