using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator.Entity.Soul
{
    public class TerritorySoul : Soul
    {
        private readonly Vector _territoryCenter;

        public TerritorySoul()
        {
            Assert.IsTrue(Application.isEditor, "Territory can be null only at test time");
            _territoryCenter = DenseVector.Build.Random(1) as Vector;
        }

        public TerritorySoul(Vector territoryCenter)
        {
            _territoryCenter = territoryCenter;
        }

        public TerritorySoul(Vector3 territoryCenter) : this(
            new DenseVector(new[] {territoryCenter.x, territoryCenter.y, (double) territoryCenter.z}))
        {
        }


        public override float Reward(State lastState, State nowState)
        {
            Assert.IsTrue(lastState.ContainsKey(State.BasicKeys.Position));
            Assert.IsTrue(nowState.ContainsKey(State.BasicKeys.Position));

            var lastDistance = Distance(_territoryCenter, lastState[State.BasicKeys.Position]);
            var currentDistance = Distance(_territoryCenter, nowState[State.BasicKeys.Position]);
            return lastDistance - currentDistance;
        }

        public override ISoulSaveData SaveAsInterface()
        {
            return new TerritorySoulSaveData(_territoryCenter);
        }
    }
}