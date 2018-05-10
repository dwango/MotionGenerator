using System.Collections.Generic;
using chainer;
using chainer.functions;
using MathNet.Numerics.LinearAlgebra;

namespace MotionGenerator.Algorithm.Reinforcement.Models
{
    public class Simple4Layer : ModelBase
    {
        private readonly Function _activation;
        public Link l1;
        public Link l2;
        public Link l3;
        private int _hiddenDimention;

        public Simple4Layer(int inputDimention, int outputDimention, int hiddenDimention, int soulCount = 1) : base(
            new Dictionary<string, Link>()
            {
                {
                    "l1",
                    new chainer.links.Linear(inSize: inputDimention, outSize: hiddenDimention, reuseAfterBackward: true)
                },
                {
                    "l2",
                    new chainer.links.Linear(inSize: hiddenDimention, outSize: hiddenDimention,
                        reuseAfterBackward: true)
                },
                {
                    "l3",
                    new chainer.links.Linear(inSize: hiddenDimention, outSize: outputDimention * soulCount,
                        reuseAfterBackward: true)
                },
            })
        {
            _hiddenDimention = hiddenDimention;
            l1 = Children["l1"];
            l2 = Children["l2"];
            l3 = Children["l3"];
//            _Params["W"] = new Variable(Matrix<float>.Build.Random(1, outputDimention * soulCount));
        }

        internal virtual Variable Activate(Variable x)
        {
            return ReLU.ForwardStatic(x);
        }


        protected override Variable ForwardImpl(Variable x)
        {
            var h = x;
            h = Activate(Children["l1"].Forward(h));
            h = Activate(Children["l2"].Forward(h));
            h = Children["l3"].Forward(h);
//            h = Multiply.ForwardStatic(h, _Params["W"]);
            return h;
        }

        public override void AlterInputDimention(int dimention)
        {
            Children["l1"] =
                new chainer.links.Linear(inSize: dimention, outSize: _hiddenDimention, reuseAfterBackward: true);
        }

        public override void AlterOutputDimention(int dimention)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Simple3Layer : Simple4Layer
    {
        public Simple3Layer(int inputDimention, int outputDimention, int hiddenDimention, int soulCount = 1) : base(
            inputDimention,
            outputDimention, hiddenDimention, soulCount)
        {
        }


        /// <summary>
        /// hidden to hiddenをなくした3層ネットワーク
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected override Variable ForwardImpl(Variable x)
        {
            var h = x;
            h = Activate(Children["l1"].Forward(h));
            h = Children["l3"].Forward(h);
//            h = Multiply.ForwardStatic(h, _Params["W"]);
            return h;
        }
    }


    public class Simple3LayerSigmoid : Simple3Layer
    {
        public Simple3LayerSigmoid(int inputDimention, int outputDimention, int hiddenDimention,
            int soulCount = 1) : base(
            inputDimention,
            outputDimention, hiddenDimention, soulCount)
        {
        }

        internal override Variable Activate(Variable x)
        {
            return Sigmoid.ForwardStatic(x);
        }
    }

    public class Simple4LayerSigmoid : Simple4Layer
    {
        public Simple4LayerSigmoid(int inputDimention, int outputDimention, int hiddenDimention) : base(inputDimention,
            outputDimention, hiddenDimention)
        {
        }

        internal override Variable Activate(Variable x)
        {
            return Sigmoid.ForwardStatic(x);
        }
    }
}