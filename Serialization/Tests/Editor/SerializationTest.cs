using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessagePack;
using NUnit.Framework;

namespace Serialization
{
    public partial class SerializationTest
    {
        public List<Type> GetAllSerializableTypes()
        {
            return Assembly.GetAssembly(typeof(MotionGeneratorSerialization)).GetTypes()
                .Where(t => t.Namespace.Contains("MotionGenerator"))
                .Where(x => x.GetCustomAttributes(typeof(MessagePackObjectAttribute), true).Length > 0)
                .ToList();
        }

        public List<Type> GetKeyAttributedFields(Type t)
        {
            var fields = t.GetFields().Where(y => y.GetCustomAttribute<KeyAttribute>(true) != null);
            return fields.Select(x => new
                {
                    Type = x.FieldType,
                    Number = x.GetCustomAttribute<KeyAttribute>(true).IntKey
                })
                .OrderBy(x => x.Number)
                .Select(x => x.Type)
                .ToList();
        }

        [Test]
        public void MessagePackObject属性クラスがSerializableである()
        {
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
            foreach (var type in GetAllSerializableTypes())
            {
                Assert.DoesNotThrow(() => { EditorTestExtensions.TrySerializingNonGeneric(type); },
                    "at " + type.FullName);
            }
        }

        [Test]
        public void MessagePackObjectクラスがFieldメンバを持たない()
        {
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
            foreach (var type in GetAllSerializableTypes())
            {
                var fields = GetKeyAttributedFields(type);
                Assert.AreEqual(0, fields.Count, type.ToString());
            }
        }

        [Test]
        public void MessagePackObject属性クラスがIALifeSerializableである()
        {
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
            foreach (var type in GetAllSerializableTypes())
            {
                var iserializable = typeof(IMotionGeneratorSerializable<>).MakeGenericType(type);
                Assert.IsTrue(iserializable.IsAssignableFrom(type), type.ToString());
            }
        }
    }
}