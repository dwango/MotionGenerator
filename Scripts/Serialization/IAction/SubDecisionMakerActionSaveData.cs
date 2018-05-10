using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class SubDecisionMakerActionSaveData : IActionSaveData, IALifeSerializable<SubDecisionMakerActionSaveData>
    {
        [Key(0)] public ActionBaseSaveData ActionBase { get; set; }
        [Key(1)] public IDecisionMakerSaveData DecisionMakerSaveData { get; set; }

        public SubDecisionMakerActionSaveData()
        {
            
        }

        public SubDecisionMakerActionSaveData(ActionBaseSaveData actionBase, IDecisionMakerSaveData decisionMakerSaveData)
        {
            DecisionMakerSaveData = decisionMakerSaveData;
            ActionBase = actionBase;
        }

        public IAction Instantiate()
        {
            return new SubDecisionMakerAction(this);
        }
    }
}