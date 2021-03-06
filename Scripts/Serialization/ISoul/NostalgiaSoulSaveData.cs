﻿using MessagePack;
using MotionGenerator.Entity.Soul;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class NostalgiaSoulSaveData : ISoulSaveData, IMotionGeneratorSerializable<NostalgiaSoulSaveData>
    {
        public ISoul Instantiate()
        {
            return new NostalgiaSoul();
        }
    }
}