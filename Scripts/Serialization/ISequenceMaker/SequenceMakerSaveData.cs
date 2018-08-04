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

        public static void AddDeserializer(string typestring, Func<byte[], ISequenceMaker> func)
        {
            deserializer[typestring] = func;
        }

        public SequenceMakerSaveData()
        {
        }

        public SequenceMakerSaveData(string typeString, byte[] saveData)
        {
            TypeString = typeString;
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