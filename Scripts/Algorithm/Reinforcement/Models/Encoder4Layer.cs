using System.Collections.Generic;
using chainer;
using chainer.functions;

namespace MotionGenerator.Algorithm.Reinforcement.Models
{
    /// <summary>
    /// 入力層が大きくて、だんだんEncodeする、一般的なモデル
    /// </summary>
    public class Encoder4Layer : ModelBase
    {
        private readonly int _hiddenDimention;

        public Encoder4Layer(int inputDimention, int outputDimention, int hiddenDimention = 8) : base(
            new Dictionary<string, Link>()
            {
                {
                    "l1",
                    new chainer.links.Linear(inSize: inputDimention, outSize: hiddenDimention * 4,
                        reuseAfterBackward: true)
                },
                {
                    "l2",
                    new chainer.links.Linear(inSize: hiddenDimention * 4, outSize: hiddenDimention * 2,
                        reuseAfterBackward: true)
                },
                {
                    "l3",
                    new chainer.links.Linear(inSize: hiddenDimention * 2, outSize: hiddenDimention,
                        reuseAfterBackward: true)
                },
                {
                    "l4",
                    new chainer.links.Linear(inSize: hiddenDimention, outSize: outputDimention,
                        reuseAfterBackward: true)
                },
            })
        {
            _hiddenDimention = hiddenDimention;
        }

        internal virtual Variable Activate(Variable x)
        {
            return Sigmoid.ForwardStatic(x);
        }

        protected override Variable ForwardImpl(Variable x)
        {
            var h = x;
            h = Activate(Children["l1"].Forward(h));
            h = Activate(Children["l2"].Forward(h));
            h = Activate(Children["l3"].Forward(h));
            h = Children["l4"].Forward(h);
            return h;
        }

        public override void AlterInputDimention(int dimention)
        {
            Children["l1"] = new chainer.links.Linear(inSize: dimention, outSize: _hiddenDimention * 4,
                reuseAfterBackward: true);
        }

        public override void AlterOutputDimention(int dimention)
        {
            throw new System.NotImplementedException();
        }
    }
}