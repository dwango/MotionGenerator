using MessagePack;

namespace MotionGenerator.Serialization
{
    public static class MotionGeneratorSerialization
    {
        public static byte[] Serialize<T>(T obj)
        {
            return LZ4MessagePackSerializer.Serialize(obj);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return LZ4MessagePackSerializer.Deserialize<T>(data);
        }

        public static T DeepClone<T>(T obj)
        {
            return DeserializeFast<T>(SerializeFast(obj));
        }

        private static byte[] SerializeFast<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        private static T DeserializeFast<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data);
        }
    }
}