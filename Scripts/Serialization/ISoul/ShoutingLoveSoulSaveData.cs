﻿using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class ShoutingLoveSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<ShoutingLoveSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new ShoutingLoveSoul();
        }
    }
}