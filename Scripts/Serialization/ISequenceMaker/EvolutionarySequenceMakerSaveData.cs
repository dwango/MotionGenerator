using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class EvolutionarySequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<EvolutionarySequenceMakerSaveData>
    {
        [Key(0)] public float Epsilon { get; set; }
        [Key(1)] public int MinimumCandidates { get; set; }
        [Key(2)] public ActionSaveData LastAction { get; set; }
        [Key(3)] public CandidateSaveData LastOutput { get; set; }
        [Key(4)] public Dictionary<string, List<CandidateSaveData>> CandidatesDict { get; set; }

        public EvolutionarySequenceMakerSaveData()
        {
        }

        public EvolutionarySequenceMakerSaveData(float epsilon,
            int minimumCandidates, ActionSaveData lastAction, CandidateSaveData lastOutput,
            Dictionary<string, List<CandidateSaveData>> candidatesDict)
        {
            Epsilon = epsilon;
            MinimumCandidates = minimumCandidates;
            LastAction = lastAction;
            LastOutput = lastOutput;
            CandidatesDict = candidatesDict;
        }

        public EvolutionarySequenceMaker Instantiate()
        {
            return new EvolutionarySequenceMaker(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}