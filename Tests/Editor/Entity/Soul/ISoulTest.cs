using System.Linq;
using NUnit.Framework;

namespace MotionGenerator.Entity.Soul
{
    public class ISoulTest
    {
        [Test]
        public void すべてのSoulが複製できる()
        {
            var targets = System.AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Namespace == this.GetType().Namespace)
                .Where(t => typeof(ISoul).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (var target in targets)
            {
                var instance = (ISoul) System.Activator.CreateInstance(target);
                var clone = instance.SaveAsInterface().Instantiate();
                Assert.AreEqual(instance.GetType(), clone.GetType());
            }
        }
    }
}