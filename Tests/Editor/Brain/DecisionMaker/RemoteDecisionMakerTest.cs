using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace MotionGenerator
{
    public class RemoteDecisionMakerTest
    {
        class DummyFetcher : JsonHttpFetcher
        {
            public DummyFetcher() : base("dummy.host")
            {
            }

            public override string Post(string endpoint, string body)
            {
                // Do Nothing
                return "{\"name\": \"test\"}";
            }
        }

        class DummyRemoteDecisionMaker : RemoteDecisionMaker
        {
            public DummyRemoteDecisionMaker()
            {
                Fetcher = new DummyFetcher();
                Actions = new List<IAction>() {LocomotionAction.GoStraight("test")};
            }
        }

        [Test]
        public void CanPassDroppedState()
        {
            var decisionMaker = new DummyRemoteDecisionMaker();
            var tmpState = new State();
            decisionMaker.DecideAction(tmpState);
        }
    }
}