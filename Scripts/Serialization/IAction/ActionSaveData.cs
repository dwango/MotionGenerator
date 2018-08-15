using System;
using System.Collections.Generic;
using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class ActionSaveData : IMotionGeneratorSerializable<ActionSaveData>
    {
        [Key(0)] public string TypeString { get; set; }
        [Key(1)] public byte[] SaveData { get; set; }

        private static Dictionary<string, Func<byte[], IAction>> deserializer =
            new Dictionary<string, Func<byte[], IAction>>();

        public static void AddDeserializer(string T, Func<byte[], IAction> func)
        {
            deserializer[T] = func;
        }

        public static void AddDeserializer<T>(Func<byte[], IAction> func) where T : IAction
        {
            deserializer[typeof(T).ToString()] = func;
        }

        private static void AddDefaultSerializers()
        {
            AddDeserializer<TurnRightAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<TurnRightActionSaveData>(baseData);
                return new TurnRightAction(saveData);
            });
            AddDeserializer<TurnLeftAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<TurnLeftActionSaveData>(baseData);
                return new TurnLeftAction(saveData);
            });
            AddDeserializer<GoForwardCoordinateAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<GoForwardCoordinateActionSaveData>(baseData);
                return new GoForwardCoordinateAction(saveData);
            });
            AddDeserializer<ShootUpwardAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<ShootUpwardActionSaveData>(baseData);
                return new ShootUpwardAction(saveData);
            });
            AddDeserializer<SubDecisionMakerAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<SubDecisionMakerActionSaveData>(baseData);
                return new SubDecisionMakerAction(saveData);
            });
            AddDeserializer<StayAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<StayActionSaveData>(baseData);
                return new StayAction(saveData);
            });
            AddDeserializer<SpinTurnAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<SpinTurnActionSaveData>(baseData);
                return new SpinTurnAction(saveData);
            });
            AddDeserializer<RestAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<RestActionSaveData>(baseData);
                return new RestAction(saveData);
            });
            AddDeserializer<HopAction>(baseData =>
            {
                var saveData = MotionGeneratorSerialization.Deserialize<HopActionSaveData>(baseData);
                return new HopAction(saveData);
            });
        }

        static ActionSaveData()
        {
            AddDefaultSerializers();
        }

        public ActionSaveData()
        {
        }

        public ActionSaveData(Type t, byte[] saveData)
        {
            TypeString = t.ToString();
            SaveData = saveData;
        }

        public IAction Instantiate()
        {
            if (!deserializer.ContainsKey(TypeString))
            {
                throw new Exception($"{TypeString} is not registered to deserializer");
            }

            return deserializer[TypeString](SaveData);
        }
    }
}