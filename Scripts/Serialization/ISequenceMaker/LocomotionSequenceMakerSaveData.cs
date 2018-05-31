using System.Collections.Generic;
using MessagePack;
using Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class LocomotionSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<LocomotionSequenceMakerSaveData>
    {
        [Key(0)] public SequenceMakerBaseSaveData SequenceMakerBase { get; set; }
        [Key(1)] public float Epsilon { get; set; }
        [Key(2)] public int MinimumCandidates { get; set; }
        [Key(3)] public float TimeScale { get; set; }
        [Key(4)] public List<LocomotionActionSaveData> LocomotionActions { get; set; }
        [Key(5)] public IActionSaveData LastAction { get; set; }
        [Key(6)] public Candidate3DSaveData LastOutput { get; set; }
        [Key(7)] public List<Candidate3DSaveData> Candidates { get; set; }
        [Key(8)] public RandomSequenceMakerSaveData RandomMaker { get; set; }
        [Key(9)] public ISequenceMakerSaveData FallbackSequenceMaker { get; set; }
        [Key(10)] public int ManipulatableDimension { get; set; }
        [Key(11)] public bool EnableTurn { get; set; }

        public LocomotionSequenceMakerSaveData()
        {
            
        }

        public LocomotionSequenceMakerSaveData(SequenceMakerBaseSaveData sequenceMakerBase, float epsilon,
            int minimumCandidates, float timeScale, List<LocomotionActionSaveData> locomotionActions,
            IActionSaveData lastAction, Candidate3DSaveData lastOutput, List<Candidate3DSaveData> candidates,
            RandomSequenceMakerSaveData randomMaker, ISequenceMakerSaveData fallbackSequenceMaker,
            int manipulatableDimension, bool enableTurn)
        {
            SequenceMakerBase = sequenceMakerBase;
            Epsilon = epsilon;
            MinimumCandidates = minimumCandidates;
            TimeScale = timeScale;
            LocomotionActions = locomotionActions;
            LastAction = lastAction;
            LastOutput = lastOutput;
            Candidates = candidates;
            RandomMaker = randomMaker;
            FallbackSequenceMaker = fallbackSequenceMaker;
            ManipulatableDimension = manipulatableDimension;
            EnableTurn = enableTurn;
        }

        public LocomotionSequenceMaker Instantiate()
        {
            return new LocomotionSequenceMaker(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}