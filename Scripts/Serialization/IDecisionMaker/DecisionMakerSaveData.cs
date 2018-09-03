using System;
using System.Collections.Generic;
using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class DecisionMakerSaveData : IMotionGeneratorSerializable<DecisionMakerSaveData>
    {
        [Key(0)] public string TypeString { get; set; }
        [Key(1)] public byte[] SaveData { get; set; }

        private static Dictionary<string, Func<byte[], IDecisionMaker>> deserializer =
            new Dictionary<string, Func<byte[], IDecisionMaker>>();

        public static void AddDeserializer(string T, Func<byte[], IDecisionMaker> func)
        {
            deserializer[T] = func;
        }

        public static void AddDeserializer<T>(Func<byte[], IDecisionMaker> func) where T : IDecisionMaker
        {
            deserializer[typeof(T).ToString()] = func;
        }

        private static void AddDefaultSerializers()
        {
            AddDeserializer<NoDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<NoDecisionMakerSaveData>(baseData);
                return new NoDecisionMaker(saveData);
            });
            AddDeserializer<RemoteDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<RemoteDecisionMakerSaveData>(baseData);
                return new RemoteDecisionMaker(saveData);
            });
            AddDeserializer<FollowPointDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<FollowPointDecisionMakerSaveData>(baseData);
                return new FollowPointDecisionMaker(saveData);
            });
            AddDeserializer<ReinforcementDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<ReinforcementDecisionMakerSaveData>(baseData);
                return new ReinforcementDecisionMaker(saveData);
            });
            AddDeserializer<FollowHighestDensityDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<FollowHighestDensityDecisionMakerSaveData>(baseData);
                return new FollowHighestDensityDecisionMaker(saveData);
            });
            AddDeserializer<HeuristicReinforcementDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<HeuristicReinforcementDecisionMakerSaveData>(baseData);
                return new HeuristicReinforcementDecisionMaker(saveData);
            });
            AddDeserializer<FollowPointOrIdleDecisionMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<FollowPointOrIdleDecisionMakerSaveData>(baseData);
                return new FollowPointOrIdleDecisionMaker(saveData);
            });
        }

        static DecisionMakerSaveData()
        {
            AddDefaultSerializers();
        }

        public DecisionMakerSaveData()
        {
        }

        public DecisionMakerSaveData(Type t, byte[] saveData)
        {
            TypeString = t.ToString();
            SaveData = saveData;
        }

        public IDecisionMaker Instantiate()
        {
            if (!deserializer.ContainsKey(TypeString))
            {
                throw new Exception($"{TypeString} is not registered to deserializer");
            }

            return deserializer[TypeString](SaveData);
        }
    }
}