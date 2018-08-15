using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class BrainSaveData : IBrainSaveData, IMotionGeneratorSerializable<BrainSaveData>
    {
        [Key(0)] public IDecisionMakerSaveData DecisionMaker { get; set; }
        [Key(1)] public ActionSaveData CurrentAction { get; set; }
        [Key(2)] public StateSaveData LastState { get; set; }
        [Key(3)] public SequenceMakerSaveData SequenceMaker { get; set; }

        public BrainSaveData()
        {
            
        }

        public BrainSaveData(IDecisionMakerSaveData decisionMaker, SequenceMakerSaveData sequenceMaker,
            ActionSaveData currentAction, StateSaveData lastState)
        {
            DecisionMaker = decisionMaker;
            SequenceMaker = sequenceMaker;
            CurrentAction = currentAction;
            LastState = lastState;
        }

        public IBrain Instantiate()
        {
            return new Brain(this);
        }
    }
}