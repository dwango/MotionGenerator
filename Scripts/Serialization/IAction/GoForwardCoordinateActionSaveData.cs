﻿using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class GoForwardCoordinateActionSaveData : IActionSaveData, IMotionGeneratorSerializable<GoForwardCoordinateActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }

        public GoForwardCoordinateActionSaveData()
        {
            
        }

        public GoForwardCoordinateActionSaveData(ActionBaseSaveData actionBase)
        {
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new GoForwardCoordinateAction(this);
        }
    }
}