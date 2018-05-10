using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

namespace MotionGenerator.Algorithm.Reinforcement.Samples
{
    public class TSVLogger
    {
        private readonly StreamWriter _writer;

        public TSVLogger()
        {
            var file = new FileInfo(string.Format("{0}/log_{1}.txt", Application.dataPath, DateTime.Now.ToString("s")));
            _writer = file.AppendText();
            Debug.Log(String.Format("write log to {0}", file.FullName));
        }

        public void Write(IEnumerable<object> datum)
        {
            _writer.WriteLine(string.Join("\t", datum.Select(x => x.ToString()).ToArray()));
        }
    }

    public class QLearningBenchmarkBase : MonoBehaviour
    {
        /// <summary>
        /// 基本的にはstateと同じactionを撮るのが正しい
        /// </summary>
        protected static float RewardFunction0(Matrix<float> state, int action)
        {
            return (int) state[0, 0] == action
                ? action
                : 0;
        }


        /// <summary>
        /// state-1のactionを撮るのが正しい
        /// </summary>
        protected static float RewardFunction1(Matrix<float> state, int action)
        {
            return (int) (state[0, 0] - 1) == action
                ? action
                : 0;
        }

        /// <summary>
        /// stateが0の時は、報酬0でも、未来のために0を選ぶように強化学習できることの確認
        /// </summary>
        protected static Matrix<float> NextState(Matrix<float> currentState, int action)
        {
            var stateValue = (int) currentState[0, 0];
            return (stateValue == 4)
                ? Matrix<float>.Build.DenseDiagonal(1, 0)
                : (
                    (stateValue == 0 && action == 0)
                        ? Matrix<float>.Build.DenseDiagonal(1, 3)
                        : Matrix<float>.Build.DenseDiagonal(1, stateValue + 1)
                );
        }
    }
}