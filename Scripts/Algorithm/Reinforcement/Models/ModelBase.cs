using System.Collections.Generic;
using System.Linq;
using chainer;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using UnityEngine.Assertions;

namespace MotionGenerator.Algorithm.Reinforcement.Models
{
    public static class MatrixExtention
    {
        public static bool IsNormalized(this Matrix<float> self)
        {
            for (var row = 0; row < self.RowCount; row++)
            {
                for (var col = 0; col < self.ColumnCount; col++)
                {
                    var val = self.At(row, col);
                    if (val < -1f || 1f < val)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }


    /// <summary>
    /// 共通インタフェース
    /// 次元の部分変更を許す
    /// </summary>
    public abstract class ModelBase : Chain
    {
        protected ModelBase(Dictionary<string, Link> children) : base(children)
        {
        }

        /// <summary>
        /// 入力次元を変える。
        /// </summary>
        /// <param name="dimention"></param>
        public abstract void AlterInputDimention(int dimention);

        /// <summary>
        /// 出力次元を変える
        /// </summary>
        /// <param name="dimention"></param>
        public abstract void AlterOutputDimention(int dimention);

        protected abstract Variable ForwardImpl(Variable x);

        public override Variable Forward(Variable x)
        {
            Debug.AssertFormat(x.Value.IsNormalized(), "inputlayer should be normalized but {0}", x.Value);
            return ForwardImpl(x);
        }
    }
}