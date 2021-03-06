﻿using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SnufflingSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<SnufflingSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new SnufflingSoul();
        }
    }
}