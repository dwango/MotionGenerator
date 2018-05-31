﻿using MessagePack;
using MotionGenerator.Entity.Soul;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class GluttonySoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<GluttonySoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new GluttonySoul();
        }
    }
}