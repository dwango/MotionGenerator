using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class FollowHighestDensityDecisionMaker : DecisionMakerBase
    {
        private const int Direction = 8;

        private readonly System.Random _random = new System.Random();

        private string _stateKey;
        private bool _isNegative;

        public FollowHighestDensityDecisionMaker(string stateKey, bool isNegative = false)
        {
            _stateKey = stateKey;
            _isNegative = isNegative;
        }

        public FollowHighestDensityDecisionMaker(FollowHighestDensityDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
            _stateKey = saveData.StateKey;
            _isNegative = saveData.IsNegative;
        }

        public new FollowHighestDensityDecisionMakerSaveData Save()
        {
            return new FollowHighestDensityDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save(),
                _stateKey, _isNegative
            );
        }

        public override DecisionMakerSaveData SaveAsInterface()
        {
            return new DecisionMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        private float CalculateDensity(Vector states, int direction, int diffuse)
        {
            var density = 0f;
            for (var i = direction - diffuse; i <= direction + diffuse * 2; i++)
            {
                var d = (float) states[(Direction + i) % Direction];
                density += d * (1 + diffuse - Mathf.Abs(i - direction));
            }
            return density;
        }

        public override IAction DecideAction(State state)
        {
            int targetDir = -1;
            if (state.ContainsKey(_stateKey) && state[_stateKey].Count == Direction)
            {
                var densities = state[_stateKey];
                var highestDensity = 0f;
                for (var i = 0; i < Direction; i++)
                {
                    var d = CalculateDensity(densities, i, diffuse: 1);
                    if (d > highestDensity)
                    {
                        highestDensity = d;
                        targetDir = i;
                    }
                }
                
                if (_isNegative && targetDir >=0)
                {
                    // いちばん高いところの反対方向あたりでもっとも低い方向を探す
                    var startDir = (targetDir + Direction / 2) % Direction;
                    var lowestDensity = Mathf.Infinity;
                    for (var i = 0; i < Direction; i++)
                    {
                        var d = CalculateDensity(densities, (startDir + i) % Direction, diffuse: 2);
                        if (d < lowestDensity)
                        {
                            lowestDensity = d;
                            targetDir = (startDir + i) % Direction;
                        }
                    }
                }
            }

            if (targetDir < 0)
            {
                // 全方向 Densityが0だった
                for (var i = 0;i < Actions.Count;i++)
                {
                    if (Actions[i].Name == "rest")
                    {
                        return Actions[i];
                    }
                }
                targetDir = _random.Next(Direction);
            }
            return Actions[targetDir];
        }
    }
}