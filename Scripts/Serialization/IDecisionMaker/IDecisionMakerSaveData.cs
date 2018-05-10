using MessagePack;

namespace MotionGenerator.Serialization
{
    [Union(0, typeof(NoDecisionMakerSaveData))]
    [Union(1, typeof(RemoteDecisionMakerSaveData))]
    [Union(2, typeof(FollowPointDecisionMakerSaveData))]
    [Union(3, typeof(ReinforcementDecisionMakerSaveData))]
    [Union(4, typeof(FollowHighestDensityDecisionMakerSaveData))]
    [Union(5, typeof(HeuristicReinforcementDecisionMakerSaveData))]
    [Union(6, typeof(FollowPointOrIdleDecisionMakerSaveData))]
    
    public interface IDecisionMakerSaveData
    {
        IDecisionMaker Instantiate();
    }
}