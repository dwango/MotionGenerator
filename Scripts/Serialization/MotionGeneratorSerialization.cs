using MessagePack;

namespace MotionGenerator.Serialization
{
    public static class MotionGeneratorSerialization
    {
        public static byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data);
        }

        public static T DeepClone<T>(T obj)
        {
            return Deserialize<T>(Serialize(obj));
        }
    }
}