using System;
using MotionGenerator.Serialization;

namespace MotionGenerator
{
    public interface IAction
    {
        string Name { get; }
        IAction Clone();
        float Reward(State lastState, State nowState);
        IActionSaveData SaveAsInterface();
    }
}