using System;
using System.Collections.Generic;
using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class SequenceMakerSaveData : IMotionGeneratorSerializable<SequenceMakerSaveData>
    {
        [Key(0)] public string TypeString { get; set; }
        [Key(1)] public byte[] SaveData { get; set; }

        private static Dictionary<string, Func<byte[], ISequenceMaker>> deserializer =
            new Dictionary<string, Func<byte[], ISequenceMaker>>();

        public static void AddDeserializer(string T, Func<byte[], ISequenceMaker> func)
        {
            deserializer[T] = func;
        }

        public static void AddDeserializer<T>(Func<byte[], ISequenceMaker> func) where T : ISequenceMaker
        {
            deserializer[typeof(T).ToString()] = func;
        }

        private static void AddDefaultSerializers()
        {
            AddDeserializer<EvolutionarySequenceMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<EvolutionarySequenceMakerSaveData>(baseData);
                return new EvolutionarySequenceMaker(saveData);
            });
            AddDeserializer<FixedSequenceMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<FixedSequenceMakerSaveData>(baseData);
                return new FixedSequenceMaker(saveData);
            });
            AddDeserializer<RandomSequenceMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<RandomSequenceMakerSaveData>(baseData);
                return new RandomSequenceMaker(saveData);
            });
            AddDeserializer<SimpleBanditSequenceMaker>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<SimpleBanditSequenceMakerSaveData>(baseData);
                return new SimpleBanditSequenceMaker(saveData);
            });
            AddDeserializer<SimpleBanditSequenceMakerRandom>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<SimpleBanditSequenceMakerRandomSaveData>(baseData);
                return new SimpleBanditSequenceMakerRandom(saveData);
            });
        }

        static SequenceMakerSaveData()
        {
            AddDefaultSerializers();
        }

        public SequenceMakerSaveData()
        {
        }

        public SequenceMakerSaveData(Type t, byte[] saveData)
        {
            TypeString = t.ToString();
            SaveData = saveData;
        }

        public ISequenceMaker Instantiate()
        {
            if (!deserializer.ContainsKey(TypeString))
            {
                throw new Exception($"{TypeString} is not registered to deserializer");
            }

            return deserializer[TypeString](SaveData);
        }
    }
}