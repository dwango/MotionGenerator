﻿using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SequenceMakerBaseSaveData : ISequenceMakerSaveData, IALifeSerializable<SequenceMakerBaseSaveData>
    {
        public SequenceMakerBaseSaveData()
        {
            
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            throw new System.NotImplementedException();
        }
    }
}