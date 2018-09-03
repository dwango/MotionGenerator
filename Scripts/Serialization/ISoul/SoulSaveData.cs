using System;
using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Entity.Soul;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class SoulSaveData : IMotionGeneratorSerializable<SoulSaveData>
    {
        [Key(0)] public string TypeString { get; set; }
        [Key(1)] public byte[] SaveData { get; set; }

        private static Dictionary<string, Func<byte[], ISoul>> deserializer =
            new Dictionary<string, Func<byte[], ISoul>>();

        public static void AddDeserializer(string T, Func<byte[], ISoul> func)
        {
            deserializer[T] = func;
        }

        public static void AddDeserializer<T>(Func<byte[], ISoul> func) where T : ISoul
        {
            deserializer[typeof(T).ToString()] = func;
        }

        public static void AddDeserializer<T>() where T : ISoul, new()
        {
            AddDeserializer<T>(_ => { return new T(); });
        }

        private static void AddDefaultSerializers()
        {
            AddDeserializer<GluttonySoul>();
            AddDeserializer<SnufflingSoul>();
            AddDeserializer<SnufflingDifferencialSoul>();
            AddDeserializer<SnufflingFleshSoul>();
            AddDeserializer<SnufflingFleshDifferencialSoul>();
            AddDeserializer<ShoutingLoveSoul>();
            AddDeserializer<ShoutingLoveDifferencialSoul>();
            AddDeserializer<NostalgiaSoul>();
            AddDeserializer<CrowdDifferencialSoul>();
            AddDeserializer<CowardSoul>();
            AddDeserializer<CowardDiffrencialSoul>();
            AddDeserializer<BumpingDifferencialSoul>();
            AddDeserializer<LazySoul>();
            AddDeserializer<FamiliarDiffrencialSoul>();
            AddDeserializer<ModerateSnufflingDifferencialSoul>();
            AddDeserializer<ModerateSnufflingFleshDifferencialSoul>();
            AddDeserializer<ModerateFamiliarDiffrencialSoul>();
            AddDeserializer<ModerateBumpingDifferencialSoul>();
            AddDeserializer<AvoidObjectSoul>();
            AddDeserializer<EnergizeSoul>();
            AddDeserializer<TerritorySoul>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<TerritorySoulSaveData>(baseData);
                return new TerritorySoul(saveData);
            });
        }

        static SoulSaveData()
        {
            AddDefaultSerializers();
        }

        public SoulSaveData()
        {
        }

        public SoulSaveData(Type t, byte[] saveData)
        {
            TypeString = t.ToString();
            SaveData = saveData;
        }

        public ISoul Instantiate()
        {
            if (!deserializer.ContainsKey(TypeString))
            {
                throw new Exception($"{TypeString} is not registered to deserializer");
            }

            return deserializer[TypeString](SaveData);
        }
    }
}