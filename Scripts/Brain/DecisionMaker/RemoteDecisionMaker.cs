using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MotionGenerator.Serialization;
using UnityEngine;

namespace MotionGenerator
{
    public class RemoteDecisionMaker : DecisionMakerBase
    {
        protected JsonHttpFetcher Fetcher;

        public State _defaultValues = new State
        {
            {
                State.BasicKeys.RelativeFoodPosition,
                new DenseVector(new double[] {State.Inf, State.Inf, State.Inf})
            },
            {
                State.BasicKeys.RelativeEnemyPosition,
                new DenseVector(new double[] {State.Inf, State.Inf, State.Inf})
            },
            {
                State.BasicKeys.RelativeObjectPosition,
                new DenseVector(new double[] {State.Inf, State.Inf, State.Inf})
            },
        };

        public RemoteDecisionMaker(string host, int port)
        {
            Fetcher = new JsonHttpFetcher(host, port);
        }

        public RemoteDecisionMaker() : this("localhost", 8080)
        {
        }

        public RemoteDecisionMaker(RemoteDecisionMakerSaveData saveData)
            : base(saveData.DecisionMakerBase)
        {
            RemoteId = saveData.RemoteId;
        }

        public new RemoteDecisionMakerSaveData Save()
        {
            return new RemoteDecisionMakerSaveData(
                (DecisionMakerBaseSaveData) base.Save(),
                RemoteId
            );
        }

        public override DecisionMakerSaveData SaveAsInterface()
        {
            return new DecisionMakerSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }

        public override void Init(List<IAction> actions)
        {
            base.Init(actions);
            Fetcher.Post(
                "init",
                JsonUtility.ToJson(
                    actions.Select(action => new Dictionary<string, string>() {{"name", action.Name}}).ToList())
            );
            DecideAction(_defaultValues);
        }

        public override void Init(IDecisionMaker abstractParent)
        {
            var parent = (RemoteDecisionMaker) abstractParent;
            var parentId = parent.RemoteId;
            Init(parent.Actions);
            RemoteId = parentId;
        }

        private string RemoteId
        {
            get
            {
                var jsonString = Fetcher.Post("save");
                return SimpleJSON.JSON.Parse(jsonString)["id"];
            }
            set
            {
                // FIXME
                // デシリアライズ時に Fetcher の IP / Port がわからないので Init できない
                Fetcher.Post(
                    "load",
                    JsonUtility.ToJson(new Dictionary<string, string>
                    {
                        {"id", value}
                    })
                );
            }
        }

        public override IAction DecideAction(State state)
        {
            var targetKeys = _defaultValues.Keys;
            var tmpState =
                new State(state.Where(v => targetKeys.Contains(v.Key)).ToDictionary(v => v.Key, v => v.Value));
            foreach (var kv in _defaultValues)
            {
                if (!tmpState.ContainsKey(kv.Key))
                {
                    tmpState[kv.Key] = kv.Value;
                }
            }

            var actionString = Fetcher.Post(
                "decideAction",
                JsonUtility.ToJson(tmpState)
            );
            var actionName = SimpleJSON.JSON.Parse(actionString)["name"];
            return Actions.First(action => action.Name.Equals(actionName));
        }

        public override void Feedback(List<float> reward)
        {
            Fetcher.Post(
                "feedback",
                JsonUtility.ToJson(reward)
            );
        }
    }
}