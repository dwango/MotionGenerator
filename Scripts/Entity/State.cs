using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class State : Dictionary<string, Vector>
    {
        public static class BasicKeys
        {
            public const string Time = "Time";
            public const string Position = "Position";
            public const string BirthPosition = "BirthPosition";
            public const string Rotation = "Rotation";
            public const string Forward = "Forward";
            public const string BulletVelocity = "BulletVelocity";
            public const string SightRange = "SightRange";
            public const string RelativeFoodPosition = "RelativeFoodPosition";
            public const string RelativeFleshFoodPosition = "RelativeFleshFoodPosition";
            public const string RelativeCreaturePosition = "RelativeCreaturePosition";
            public const string RelativeEnemyPosition = "RelativeEnemyPosition";
            public const string RelativeTribePosition = "RelativeTribePosition";
            public const string RelativeObjectPosition = "RelativeObjectPosition";
            public const string RelativeStrangerPosition = "RelativeStrangerPosition";
            public const string PhysicalImpact = "PhysicalImpact";
            public const string OrganEnergy = "OrganEnergy";
            public const string EatableEnergy = "EatableEnergy";
            public const string ManipulatorEnergyConsumption = "ManipulatorEnergyConsumption";
            public const string AverageManipulatorEnergyConsumption = "AverageManipulatorEnergyConsumption";
            public const string HeightDifferenceInAction = "HeightDifferenceInAction";
            public const string TileAngleEachDirection = "TileAngleEachDirection";
            public const string TileAttribute = "TileAttribute";
            public const string TotalFoodCountEachDirection = "TotalFoodCountEachDirection";
            public const string TotalFoodEnergyEachDirection = "TotalFoodEnergyEachDirection";
            public const string TotalFleshFoodCountEachDirection = "TotalFleshFoodCountEachDirection";
            public const string TotalEnemyCountEachDirection = "TotalEnemyCountEachDirection";
            public const string TotalEnemyEnergyEachDirection = "TotalEnemyEnergyEachDirection";
            public const string TotalCreatureCountEachDirection = "TotalCreatureCountEachDirection";
            public const string TotalCreatureEnergyEachDirection = "TotalCreatureEnergyEachDirection";
            public const string TotalTribeCountEachDirection = "TotalTribeCountEachDirection";
            public const string TotalTribeEnergyEachDirection = "TotalTribeEnergyEachDirection";
            public const string TotalObjectCountEachDirection = "TotalObjectCountEachDirection";
            public const string TotalStrangerCountEachDirection = "TotalStrangerCountEachDirection";
            public const string TotalStrangerEnergyEachDirection = "TotalStrangerEnergyEachDirection";
            public const string RelativeObservedTilePosition = "RelativeObservedTilePosition";
            public const string WalkMotion = "WalkMotion";
            public const string AvoidLearning = "AvoidLearning"; // なにかのセンサーの状態が大きく変わって，学習をやめたい時 > 0
        }

        public const float Inf = 1000f;


        public State()
        {
        }

        public State(Dictionary<string, Vector> dictionary) : base(dictionary)
        {
        }

        public State(StateSaveData saveData)
        {
            foreach (var kv in saveData.Values)
            {
                Add(kv.Key, new DenseVector(kv.Value));
            }
        }

        public StateSaveData Save()
        {
            var dict = new Dictionary<string, double[]>();
            foreach (var kv in this)
            {
                dict.Add(kv.Key, kv.Value.ToArray());
            }

            return new StateSaveData(
                dict
            );
        }

        public void Set(string key, Vector3 v)
        {
            var p = this[key];
            p[0] = v.x;
            p[1] = v.y;
            p[2] = v.z;
        }

        public Vector3 GetAsVector3(string key)
        {
            var p = this[key];
            return new Vector3((float) p[0], (float) p[1], (float) p[2]);
        }

        public void Set(string key, Quaternion q)
        {
            var p = this[key];
            p[0] = q.x;
            p[1] = q.y;
            p[2] = q.z;
            p[3] = q.w;
        }

        public Quaternion GetAsQuaternion(string key)
        {
            var p = this[key];
            return new Quaternion((float) p[0], (float) p[1], (float) p[2], (float) p[3]);
        }

        public void Set(string key, double d)
        {
            var p = this[key];
            var count = p.Count;
            for (var i = 0; i < count; i++)
            {
                p[i] = d;
            }
        }

        public float GetAsFloat(string key)
        {
            return (float) this[key][0];
        }

        public double GetAsDouble(string key)
        {
            return this[key][0];
        }

        public void FillZero()
        {
            foreach (var kv in this)
            {
                var count = kv.Value.Count;
                for (var i = 0; i < count; i++)
                {
                    this[kv.Key][i] = 0;
                }
            }
        }

        // fromにある内容を取り込む。keyが重複していたらvalueを加算する。
        public void MergeFrom(State otherState)
        {
            foreach (var kv in otherState)
            {
                var valueCount = kv.Value.Count;

                Vector thisValue;
                TryGetValue(kv.Key, out thisValue);
                if (thisValue == null || thisValue.Count != valueCount)
                {
                    thisValue = this[kv.Key] = new DenseVector(valueCount);
                }

                for (var i = 0; i < valueCount; i++)
                {
                    thisValue[i] += kv.Value[i];
                }
            }
        }

        // fromの内容ですべて自分を置き換える。fromにないkeyは削除される。
        public void ReplaceFrom(State otherState)
        {
            FillZero();
            MergeFrom(otherState);

            var diff = Count - otherState.Count;
            if (diff > 0)
            {
                var delCount = 0;
                var delKeys = new string[diff];
                foreach (var kv in this)
                {
                    if (otherState.ContainsKey(kv.Key) == false)
                    {
                        delKeys[delCount] = kv.Key;
                        delCount++;
                    }
                }

                for (var i = 0; i < delCount; i++)
                {
                    Remove(delKeys[i]);
                }
            }
        }
    }
}