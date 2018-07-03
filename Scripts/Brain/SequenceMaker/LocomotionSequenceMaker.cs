using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MotionGenerator.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator
{
    public class LocomotionSequenceMaker : SequenceMakerBase
    {
        private float _epsilon; //epsilon-greedy
        private int _minimumCandidates;
        private readonly float _timeScale;
        private List<LocomotionAction> _locomotionActions;
        private List<IAction> _actions;
        private IAction _lastAction;
        private Candidate3D _lastOutput;
        private List<Candidate3D> _candidates;
        private RandomSequenceMaker _randomMaker;
        private readonly ISequenceMaker _fallbackSequenceMaker;
        private MersenneTwister _randomGenerator;
        private int _manipulatableDimension;
        private bool _lastDidFallbacked;

        private const int maxCandidates = 128; // 保持するモーションの最大数
        private const float predictionTime = 2.5f; // 角度を見積もる時間幅
        private const int NumTriedCutoffThreshold = 5; // 得意な方向を見つけるときに、偶然に移動距離が大きいモーションを除外するための試行回数の足切り
        private readonly bool _enableTurn;

        private class ImportantCandidates
        {
            public Candidate3D MaxMagunitudeCandidate;
            public Candidate3D MaxDirectionalMagunitudeCandidate;
            public Candidate3D UnexecutedCandidate;
        }

        /// <param name="epsilon"></param>
        /// <param name="minimumCandidates"></param>
        /// <param name="timeScale"></param>
        /// <param name="fallbackSequenceMaker">方向以外のActionを扱うようのMotionMaker</param>
        public LocomotionSequenceMaker(float epsilon, int minimumCandidates, float timeScale,
            ISequenceMaker fallbackSequenceMaker, bool enableTurn = false)
        {
            _epsilon = epsilon;
            _minimumCandidates = minimumCandidates;
            _timeScale = timeScale;
            _fallbackSequenceMaker = fallbackSequenceMaker;
            _lastAction = LocomotionAction.GoStraight("");
            _lastOutput = new Candidate3D(new List<MotionSequence>());
            _enableTurn = enableTurn;
        }

        public LocomotionSequenceMaker(LocomotionSequenceMakerSaveData saveData)
            : base(saveData.SequenceMakerBase)
        {
            _epsilon = saveData.Epsilon;
            _minimumCandidates = saveData.MinimumCandidates;
            _timeScale = saveData.TimeScale;
            _locomotionActions = saveData.LocomotionActions.Select(x => x.InstantiateLocomotionAction()).ToList();
            _lastAction = saveData.LastAction.Instantiate();
            _lastOutput = new Candidate3D(saveData.LastOutput);
            _candidates = saveData.Candidates.Select(x => new Candidate3D(x)).ToList();
            _randomMaker = saveData.RandomMaker.Instantiate();
            _fallbackSequenceMaker = saveData.FallbackSequenceMaker.Instantiate();
            _manipulatableDimension = saveData.ManipulatableDimension;
            _enableTurn = saveData.EnableTurn;
        }

        public new LocomotionSequenceMakerSaveData Save()
        {
            // TODO: Candidateを削減して保存

            return new LocomotionSequenceMakerSaveData(
                base.Save(),
                _epsilon,
                _minimumCandidates,
                _timeScale,
                _locomotionActions.Select(x => x.Save()).ToList(),
                _lastAction.SaveAsInterface(),
                _lastOutput.Save(),
                _candidates.Select(x => x.Save()).ToList(),
                _randomMaker.Save(),
                _fallbackSequenceMaker.SaveAsInterface(),
                _manipulatableDimension,
                _enableTurn
            );
        }

        public override ISequenceMakerSaveData SaveAsInterface()
        {
            return Save();
        }

        public override void Init(List<IAction> actions, List<int> manipulationDimensions)
        {
            _manipulatableDimension = manipulationDimensions.Sum();
            _actions = actions;
            _locomotionActions = actions
                .Where(x => x is LocomotionAction)
                .Cast<LocomotionAction>()
                .ToList();

            var nonLocomotionActions = actions
                .Where(action => !(action is LocomotionAction))
                .ToList();
            _fallbackSequenceMaker.Init(nonLocomotionActions, manipulationDimensions);

            _randomMaker = new RandomSequenceMaker(MaxSequenceLength * _timeScale, 1, 3);
            _randomMaker.Init(new List<IAction> {actions[0]}, manipulationDimensions);
            _candidates = new List<Candidate3D>(_minimumCandidates);
            for (var i = 0; i < _candidates.Capacity; i++)
            {
                _candidates.Add(new Candidate3D(_randomMaker.GenerateSequence(actions[0])));
            }
            _randomGenerator = new MersenneTwister();
        }

        public override void Init(ISequenceMaker abstrctParent)
        {
            var parent = (LocomotionSequenceMaker) abstrctParent;
            _fallbackSequenceMaker.Init(parent._fallbackSequenceMaker);
            _actions = parent._actions;
            _epsilon = parent._epsilon;
            _locomotionActions = parent._locomotionActions;
            _minimumCandidates = parent._minimumCandidates;
            _randomMaker = parent._randomMaker;
            _candidates = parent._candidates.Select(candidate => new Candidate3D(candidate)).ToList();
            _randomGenerator = parent._randomGenerator;
            _manipulatableDimension = parent._manipulatableDimension;
        }

        public override void Restore(List<IAction> actions, List<int> manipulationDimensions)
        {
            _randomGenerator = new MersenneTwister();
            _actions = actions;
            _locomotionActions = actions
                .Where(x => x is LocomotionAction)
                .Cast<LocomotionAction>()
                .ToList();
            _randomMaker.Restore(new List<IAction> {actions[0]}, manipulationDimensions);
            _fallbackSequenceMaker.Restore(actions.Where(x => !(x is LocomotionAction)).ToList(), manipulationDimensions);
        }

        public override bool NeedToAlterManipulatables(List<int> manipulationDimensions)
        {
            return _manipulatableDimension != manipulationDimensions.Sum();
        }

        public override void AlterManipulatables(List<int> manipulationDimensions)
        {
            _fallbackSequenceMaker.AlterManipulatables(manipulationDimensions);
            Init(_actions, manipulationDimensions);
        }
        
        private static ImportantCandidates GetImportantCandidates(
            List<Candidate3D> candidates, Vector3 dir)
        {
            var importantCandidates = new ImportantCandidates();
            // ここのブロックはCandidate3Dがアップデータされたときに差分変更すれば消せる。
            var maxMagunitude = float.MinValue;
            var maxDirectionalMagunitude = float.MinValue;
            foreach (var candiate in candidates)
            {
                if (candiate.NumTried == 0)
                {
                    importantCandidates.UnexecutedCandidate = candiate;
                }

                // 移動距離が最も大きいモーションを保持する
                var v = candiate.Mean.magnitude;
                if (maxMagunitude < v && candiate.NumTried >= NumTriedCutoffThreshold) 
                {
                    maxMagunitude = v;
                    importantCandidates.MaxMagunitudeCandidate = candiate;
                }

                // 目標方向成分の移動量が最も大きいモーションを保持する
                v = Vector3.Dot(candiate.Mean, dir);
                if (v > maxDirectionalMagunitude)
                {
                    maxDirectionalMagunitude = v;
                    importantCandidates.MaxDirectionalMagunitudeCandidate = candiate;
                }
            }
            return importantCandidates;
        }

        private static Candidate3D SelectCandidateWithRotationPenalty(
            List<Candidate3D> candidates, Vector3 dir, ImportantCandidates inportantCandidates)
        {
            // 得意な方向がなければ、目標方向成分の移動量が最も大きいモーションを選択
            if (inportantCandidates.MaxMagunitudeCandidate == null)
            {
                return inportantCandidates.MaxDirectionalMagunitudeCandidate;
            }

            // 得意な方向に身体を回転できる、かつ、目標方向に進めるモーションをヒューリスティックに選択
            var selectedCandidate = inportantCandidates.MaxDirectionalMagunitudeCandidate;
            var targetRotation =
                Quaternion.FromToRotation(inportantCandidates.MaxMagunitudeCandidate.Mean, dir).eulerAngles.y; // 最も得意な方向から目標方向への角度
            var maxScore = float.MinValue;
            foreach (var candiate in candidates)
            {
                var meanMovementVectorDotDirrection = Vector3.Dot(candiate.Mean, dir);

                // 暫定的に、目標地点から離れる動きは除外（目標地点までに距離を考慮しないと移動距離が評価できないことと、壁にぶつかりスタックする現象を避けるため）
                if (meanMovementVectorDotDirrection < 0) continue;

                // x秒後の期待回転角と得意な角度の差をみる。回転量が多い場合は、遠回りして目標角に近くなる場合もある。（例：少し右を向けばいいのだけど、右を向く動きがないので、左に大回りするなど）
                var ang = Mathf.DeltaAngle(targetRotation, candiate.AxisYAngularVelocity * predictionTime);

                // 角度に比例したペナルティを設定。（cosでも良さそうだが、微小なズレも評価したいため線形で）
                var discountRatio = Mathf.Max(0, 1 - Mathf.Abs(ang) / 180f);

                var score = meanMovementVectorDotDirrection * discountRatio;
                if (maxScore < score)
                {
                    maxScore = score;
                    selectedCandidate = candiate;
                }
            }

            // 回転量を可視化
//            if (maxScore != float.MinValue)
//            {
//                const float lengthPower = 5;
//                foreach (var candiate in candidates)
//                {
//                    var duaration = 0.4f;
//                    if (selectedCandidate == candiate)
//                    {
//                        duaration *= 2;
//                        Debug.DrawLine(CentralBody.transform.position,
//                            CentralBody.transform.position +
//                            CentralBody.transform.rotation * maxMagunitudeCandidate.Mean.normalized * lengthPower,
//                            Color.cyan, duaration, false); // from
//                        Debug.DrawLine(
//                            CentralBody.transform.position +
//                            CentralBody.transform.rotation * maxMagunitudeCandidate.Mean.normalized * lengthPower,
//                            CentralBody.transform.position +
//                            Quaternion.Euler(0,
//                                candiate.DirMean * Benchmark.BenchmarkRecorder.InitialForceRange * 1000f, 0) *
//                            CentralBody.transform.rotation * maxMagunitudeCandidate.Mean.normalized * lengthPower,
//                            Color.cyan, duaration, false); // to
//                    }
//                }
//            }
//            foreach (var candiate in candidates)
//            {
//                var color = Color.white;
//                var duaration = 0.4f;
//                var length = 1f;
//                if (selectedCandidate == candiate)
//                {
//                    color = Color.red;
//                    duaration *= 2;
//                }
//                Debug.DrawLine(CentralBody.transform.position,
//                    CentralBody.transform.position + CentralBody.transform.rotation * candiate.Mean * length, color,
//                    duaration, false);
//            }

            return selectedCandidate;
        }

        
        public static Candidate3D SelectCandidate(List<Candidate3D> candidates, LocomotionAction locomotionAction, bool enableTurn)
        {
            var importantCandidates = GetImportantCandidates(candidates, locomotionAction.Direction);
            if (importantCandidates.UnexecutedCandidate != null)
            {
                // 試したことないモーションは無条件で選択する
                return importantCandidates.UnexecutedCandidate;
            }
            else if (enableTurn)
            {
                return SelectCandidateWithRotationPenalty(candidates, locomotionAction.Direction, importantCandidates);
            }
            else 
            {
                // 回転考慮しないなら、目標方向成分が最大のCandidateを選択する
                return importantCandidates.MaxDirectionalMagunitudeCandidate;
            }
        }

        public override List<MotionSequence> GenerateSequence(IAction action)
        {
            var locomotionAction = action as LocomotionAction;
            _lastDidFallbacked = (locomotionAction == null);
            if (!_lastDidFallbacked)
            {
                _lastOutput = SelectCandidate(_candidates, locomotionAction, _enableTurn);
                _lastAction = action;
                return _lastOutput.Value;
            }
            else
            {
                return _fallbackSequenceMaker.GenerateSequence(action);
            }
        }

        public override void Feedback(float reward, State lastState, State currentState)
        {
            if (!_lastDidFallbacked)
            {
                var lastPosition = lastState.GetAsVector3(State.BasicKeys.Position);
                var lastRotation = lastState.GetAsQuaternion(State.BasicKeys.Rotation);
                var lastTime = lastState.GetAsDouble(State.BasicKeys.Time);
                var currentTime = currentState.GetAsDouble(State.BasicKeys.Time);

                var currentPosition = currentState.GetAsVector3(State.BasicKeys.Position);
                var currentRotation = currentState.GetAsQuaternion(State.BasicKeys.Rotation);

                var movement = Quaternion.Inverse(lastRotation) * (currentPosition - lastPosition) /
                               (float) (currentTime - lastTime);
                var rotation = Mathf.DeltaAngle(lastRotation.eulerAngles.y, currentRotation.eulerAngles.y)
                               / (float) (currentTime - lastTime);
                _lastOutput.Update(movement, rotation);
                Maintain(_lastAction);
            }
            else
            {
                _fallbackSequenceMaker.Feedback(reward, lastState, currentState);
            }
        }

        private readonly ContinuousUniform _maintainRandom = new ContinuousUniform(0, 1.0, new MersenneTwister(0));

        // 移動にも回転にも使えないモーションを削除
        // x-z平面上で、セントラルボディを中心として放射状に８方向で等分し、それぞれの領域でモーションを性能順に並べて上位X個を残す
        private List<Candidate3D> DeleteBadMotions(List<Candidate3D> candidates)
        {
            const int divisionNum = 8; // x-z平面上の放射状の分割数
            const int remaindSize = 4; // 性能順に並べたとき、いくつのモーションを残すか

            var distancesByDir = new List<Dictionary<int, float>>();
            var angularVelocitiesByDir = new List<Dictionary<int, float>>();
            var unitDirections = new List<Vector3>();
            for (var dir = 0; dir < divisionNum; dir++)
            {
                distancesByDir.Add(new Dictionary<int, float>());
                angularVelocitiesByDir.Add(new Dictionary<int, float>());
                unitDirections.Add(Quaternion.Euler(0, 360f / divisionNum * dir, 0) * Vector3.forward);
            }
            var deleteFlag = new bool[candidates.Count];
            var directionIds = new int[candidates.Count];

            // 保持しているすべてのモーションを各単位方向に割り振る
            for (var i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].NumTried == 0) continue;
                var candidate = candidates[i];

                float eulerAnglesY;
                if (candidate.Mean.magnitude == 0)
                {
                    eulerAnglesY = 0;
                }
                else
                {
                    eulerAnglesY = Quaternion.LookRotation(candidate.Mean).eulerAngles.y;
                }
                const float deltaAngleY = 360f / divisionNum / 2f; // 各方向を中心に等分割を簡単に計算するために座標を回転させる量
                var directionId = (int) Mathf.Floor((eulerAnglesY + deltaAngleY) / (360f / divisionNum)) % divisionNum;
                distancesByDir[directionId].Add(i, Vector3.Dot(candidate.Mean, unitDirections[directionId]));
                angularVelocitiesByDir[directionId].Add(i, candidate.AxisYAngularVelocity);
                deleteFlag[i] = true;
                directionIds[i] = directionId;
            }

            // 各領域ごとに、性能の良いCandidateは削除フラグを解除
            for (var dir = 0; dir < divisionNum; dir++)
            {
                int count;

                // 移動量の大きいCandidateは残す
                count = 0;
                foreach (var item in distancesByDir[dir].OrderByDescending(x => x.Value))
                {
                    if (count >= remaindSize) break;
                    count++;
                    deleteFlag[item.Key] = false;
                }
                
                // 回転量が大きいCandidateは残す
                if (_enableTurn)
                {
                    count = 0;
                    foreach (var item in angularVelocitiesByDir[dir].OrderByDescending(x => x.Value))
                    {
                        if (count >= remaindSize) break;
                        count++;
                        deleteFlag[item.Key] = false;
                    }
                    
                    // 上記とは逆方向の回転量が大きいCandidateは残す
                    count = 0;
                    foreach (var item in angularVelocitiesByDir[dir].OrderBy(x => x.Value))
                    {
                        if (count >= remaindSize) break;
                        count++;
                        deleteFlag[item.Key] = false;
                    }
                }
            }
            
            // 削除フラグがついていないCandidateを集めて、配列を作り直す
            var newCandidates = candidates.Where((candidate, index) => !deleteFlag[index]).ToList();

            // モーションの総数は、下の値以下になってるはず
            if (_enableTurn)
            {
                Assert.IsTrue(newCandidates.Count <= divisionNum * remaindSize * 3);
            }
            else
            {
                Assert.IsTrue(newCandidates.Count <= divisionNum * remaindSize * 1);
            }
//            Debug.Log(System.String.Format("number of motions: {0} -> {1}, reduction ratio:{2}", candidates.Count,
//                newCandidates.Count, (candidates.Count - newCandidates.Count) / (float) maxCandidates));

            return newCandidates;
        }

        private void Maintain(IAction action)
        {
            if (_maintainRandom.Sample() < 0.3f * _epsilon)
            {
                if (_candidates.Count >= maxCandidates)
                {
                    _candidates = DeleteBadMotions(_candidates);
                }
                if (_candidates.Count >= maxCandidates)
                {
                    Debug.LogWarning(
                        "DeleteBadMotionsでモーションが消されてない。DeleteBadMotionsの基準を厳しくして、よりモーションが削除されるようにしたほうがいい");
                    return;
                }

                var similar = _randomMaker.GenerateSimilarSequence(action, _lastOutput.Value, 0.3f * _epsilon, false);

                // Action継続時間を可変にし、Evolutionaryで時間スケールの伸縮
                var maxScaleFactor = 1.2f;
                var minScaleFactor = 1f / maxScaleFactor;
                var logMinScaleFactor = Mathf.Log(minScaleFactor);
                var logMaxScaleFactor = Mathf.Log(maxScaleFactor);
                var scaleFactor = Mathf.Exp((float) _randomGenerator.NextDouble() *
                                              (logMaxScaleFactor - logMinScaleFactor) + logMinScaleFactor);
                similar = _randomMaker.ChangeTimeScale(action, scaleFactor, similar);

                _candidates.Add(new Candidate3D(similar));
            }
        }
    }
}