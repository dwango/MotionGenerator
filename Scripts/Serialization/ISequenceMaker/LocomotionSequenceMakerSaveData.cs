using System.Collections.Generic;
using MessagePack;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class LocomotionSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<LocomotionSequenceMakerSaveData>
    {
//        [Key(0)] public SequenceMakerBaseSaveData SequenceMakerBase { get; set; }
        [Key(1)] public float Epsilon { get; set; }
        [Key(2)] public int MinimumCandidates { get; set; }
        [Key(3)] public float TimeScale { get; set; }
        [Key(4)] public List<LocomotionActionSaveData> LocomotionActions { get; set; }
//        [Key(5)] public IActionSaveData LastAction { get; set; }
        [Key(6)] public Candidate3DSaveData LastOutput { get; set; }
        [Key(7)] public List<Candidate3DSaveData> Candidates { get; set; }
        [Key(8)] public RandomSequenceMakerSaveData RandomMaker { get; set; }
//        [Key(9)] public ISequenceMakerSaveData FallbackSequenceMaker { get; set; }
        [Key(10)] public int ManipulatableDimension { get; set; }
        [Key(11)] public bool EnableTurn { get; set; }
        [Key(12)] public float ConsumptionEnergyCoef { get; set; }
        [Key(13)] public float ConsumptionEnergyPenaltyWeight { get; set; }
        [Key(14)] public LocomotionSequenceMaker.CandidatesType LastCandidatesType { get; set; }
        [Key(15)] public List<Candidate3DSaveData> CandidatesHigher { get; set; }
        [Key(16)] public List<Candidate3DSaveData> CandidatesLower { get; set; }
        [Key(17)] public List<Candidate3DSaveData> CandidatesWalking { get; set; }
        [Key(18)] public float WalkingStep { get; set; }
        [Key(19)] public SequenceMakerSaveData FallbackSequenceMaker { get; set; }
        

        public LocomotionSequenceMakerSaveData()
        {
            
        }

        public LocomotionSequenceMakerSaveData(float epsilon,
            int minimumCandidates, float timeScale, List<LocomotionActionSaveData> locomotionActions,
            Candidate3DSaveData lastOutput, LocomotionSequenceMaker.CandidatesType lastCandidatesType,
            List<Candidate3DSaveData> candidates, List<Candidate3DSaveData> candidatesHigher,
            List<Candidate3DSaveData> candidatesLower,  List<Candidate3DSaveData> candidatesWalking,
            RandomSequenceMakerSaveData randomMaker, SequenceMakerSaveData fallbackSequenceMaker,
            int manipulatableDimension, bool enableTurn,
            float consumptionEnergyCoef, float consumptionEnergyPenaltyWeight, float walkingStep)
        {
            Epsilon = epsilon;
            MinimumCandidates = minimumCandidates;
            TimeScale = timeScale;
            LocomotionActions = locomotionActions;
            LastOutput = lastOutput;
            LastCandidatesType = lastCandidatesType;
            Candidates = candidates;
            CandidatesHigher = candidatesHigher;
            CandidatesLower = candidatesLower;
            CandidatesWalking = candidatesWalking;
            RandomMaker = randomMaker;
            FallbackSequenceMaker = fallbackSequenceMaker;
            ManipulatableDimension = manipulatableDimension;
            EnableTurn = enableTurn;
            ConsumptionEnergyCoef = consumptionEnergyCoef;
            ConsumptionEnergyPenaltyWeight = consumptionEnergyPenaltyWeight;
            WalkingStep = walkingStep;
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