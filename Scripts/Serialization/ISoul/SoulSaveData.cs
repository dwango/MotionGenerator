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

        private static void AddDefaultSerializers()
        {
            AddDeserializer<GluttonySoul>(_ =>
            {
                return new GluttonySoul();
            });
            AddDeserializer<SnufflingSoul>(_ =>
            {
                return new SnufflingSoul();
            });
            AddDeserializer<SnufflingDifferencialSoul>(_ =>
            {
                return new SnufflingDifferencialSoul();
            });
            AddDeserializer<SnufflingFleshSoul>(_ =>
            {
                return new SnufflingFleshSoul();
            });
            AddDeserializer<SnufflingFleshDifferencialSoul>(_ =>
            {
                return new SnufflingFleshDifferencialSoul();
            });
            AddDeserializer<ShoutingLoveSoul>(_ =>
            {
                return new ShoutingLoveSoul();
            });
            AddDeserializer<ShoutingLoveDifferencialSoul>(_ =>
            {
                return new ShoutingLoveDifferencialSoul();
            });
            AddDeserializer<TerritorySoul>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<TerritorySoulSaveData>(baseData);
                return new TerritorySoul(saveData);
            });
            AddDeserializer<NostalgiaSoul>(_ =>
            {
                return new NostalgiaSoul();
            });
            AddDeserializer<CrowdDifferencialSoul>(_ =>
            {
                return new CrowdDifferencialSoul();
            });
            AddDeserializer<CowardSoul>(_ =>
            {
                return new CowardSoul();
            });
            AddDeserializer<CowardDiffrencialSoul>(_ =>
            {
                return new CowardDiffrencialSoul();
            });
            AddDeserializer<BumpingDifferencialSoul>(_ =>
            {
                return new BumpingDifferencialSoul();
            });
            AddDeserializer<LazySoul>(_ =>
            {
                return new LazySoul();
            });
            AddDeserializer<FamiliarDiffrencialSoul>(_ =>
            {
                return new FamiliarDiffrencialSoul();
            });
            AddDeserializer<ModerateSnufflingDifferencialSoul>(_ =>
            {
                return new ModerateSnufflingDifferencialSoul();
            });
            AddDeserializer<ModerateSnufflingFleshDifferencialSoul>(_ =>
            {
                return new ModerateSnufflingFleshDifferencialSoul();
            });
            AddDeserializer<ModerateFamiliarDiffrencialSoul>(_ =>
            {
                return new ModerateFamiliarDiffrencialSoul();
            });
            AddDeserializer<ModerateBumpingDifferencialSoul>(_ =>
            {
                return new ModerateBumpingDifferencialSoul();
            });
            AddDeserializer<AvoidObjectSoul>(_ =>
            {
                return new AvoidObjectSoul();
            });
            AddDeserializer<EnergizeSoul>(_ =>
            {
                return new EnergizeSoul();
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