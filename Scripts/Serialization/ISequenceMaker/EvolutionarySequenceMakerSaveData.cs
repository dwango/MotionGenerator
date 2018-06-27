using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class EvolutionarySequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<EvolutionarySequenceMakerSaveData>
    {
        [Key(0)] public SequenceMakerBaseSaveData SequenceMakerBase { get; set; }
        [Key(1)] public float Epsilon { get; set; }
        [Key(2)] public int MinimumCandidates { get; set; }
        [Key(3)] public IActionSaveData LastAction { get; set; }
        [Key(4)] public CandidateSaveData LastOutput { get; set; }
        [Key(5)] public Dictionary<string, List<CandidateSaveData>> CandidatesDict { get; set; }
        [Key(6)] public Dictionary<string, RandomSequenceMakerSaveData> RandomMakerDict { get; set; }

        public EvolutionarySequenceMakerSaveData()
        {
            
        }

        public EvolutionarySequenceMakerSaveData(SequenceMakerBaseSaveData sequenceMakerBase, float epsilon,
            int minimumCandidates, IActionSaveData lastAction, CandidateSaveData lastOutput,
            Dictionary<string, List<CandidateSaveData>> candidatesDict,
            Dictionary<string, RandomSequenceMakerSaveData> randomMakerDict)
        {
            SequenceMakerBase = sequenceMakerBase;
            Epsilon = epsilon;
            MinimumCandidates = minimumCandidates;
            LastAction = lastAction;
            LastOutput = lastOutput;
            CandidatesDict = candidatesDict;
            RandomMakerDict = randomMakerDict;
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