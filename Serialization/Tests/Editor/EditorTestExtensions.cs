using System;
using System.Linq;
using System.Reflection;

namespace MotionGenerator.Serialization
{
    public static class EditorTestExtensions
    {
        public static byte[] SerializeByMsgPack<T>(T src)
        {
            return MotionGeneratorSerialization.Serialize(src);
        }

        public static T DeepCloneByMsgPack<T>(T src)
        {
            return MotionGeneratorSerialization.DeepClone(src);
        }

        public static T InvokeGetField<T>(object target, Type type, string memberName)
        {
            var member = type.GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T) member.GetValue(target);
        }

        public static void InvokeSetField<T>(object target, Type type, string memberName, T val)
        {
            var member = type.GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic);
            member.SetValue(target, val);
        }

        public static T InvokeMethod<T>(object target, Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T) method.Invoke(target, null);
        }

        public static void TrySerializingNonGeneric(Type t)
        {
            typeof(EditorTestExtensions)
                .GetMethods()
                .Single(x => x.Name == "TrySerializing")
                .MakeGenericMethod(t)
                .Invoke(null, null);
        }

        public static void TrySerializing<T>()
        {
            SerializeByMsgPack<T>(default(T));
        }
    }
}