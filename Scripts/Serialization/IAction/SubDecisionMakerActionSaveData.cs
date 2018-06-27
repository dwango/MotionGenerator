using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public class SubDecisionMakerActionSaveData : IActionSaveData, IMotionGeneratorSerializable<SubDecisionMakerActionSaveData>
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