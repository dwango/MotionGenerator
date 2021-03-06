﻿using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class FollowPointOrIdleDecisionMakerSaveData : IDecisionMakerSaveData,
        IMotionGeneratorSerializable<FollowPointOrIdleDecisionMakerSaveData>
    {
        [Key(0)] public DecisionMakerBaseSaveData DecisionMakerBase { get; set; }
        [Key(1)] public List<string> StateKeys { get; set; }
        [Key(2)] public bool IsNegative { get; set; }
        [Key(3)] public float StayableDistance { get; set; }
        [Key(4)] public float NearbyDistance { get; set; }

        public FollowPointOrIdleDecisionMakerSaveData()
        {
        }

        public FollowPointOrIdleDecisionMakerSaveData(DecisionMakerBaseSaveData decisionMakerBase,
            List<string> stateKeys, bool isNegative, float stayableDistance, float nearbyDistance)
        {
            DecisionMakerBase = decisionMakerBase;
            StateKeys = stateKeys;
            IsNegative = isNegative;
            StayableDistance = stayableDistance;
            NearbyDistance = nearbyDistance;
        }

        public IDecisionMaker Instantiate()
        {
            return new FollowPointOrIdleDecisionMaker(this);
        }
    }
}