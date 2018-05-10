using System;
using System.Reflection;

namespace MotionGenerator.Tests
{
    public static class TestHelper
    {
        public static FieldInfo GetFieldInfo(Type t, string methodName)
        {
            return t.GetField(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static object GetFieldValue<T>(T target, string methodName)
        {
            return GetFieldInfo(typeof(T), methodName).GetValue(target);
        }

    }
}