using System.Collections.Generic;
using MessagePack;
using MotionGenerator.Serialization;

namespace MotionGenerator.Serialization
{
    [MessagePackObject]
    public sealed class SimpleBanditSequenceMakerSaveData : ISequenceMakerSaveData, IMotionGeneratorSerializable<SimpleBanditSequenceMakerSaveData>
    {
        [Key(0)] public float Epsilon { get; set; }
        [Key(1)] public int MinimumCandidates { get; set; }
        [Key(2)] public ActionSaveData LastAction { get; set; }
        [Key(3)] public CandidateSaveData LastOutput { get; set; }
        [Key(4)] public Dictionary<string, List<CandidateSaveData>> CandidatesDict { get; set; }
        [Key(5)] public Dictionary<string, RandomSequenceMakerSaveData> RandomMakerDict { get; set; }
        [Key(6)] public int NumControlPoints { get; set; }

        public SimpleBanditSequenceMakerSaveData()
        {
            
        }

        public SimpleBanditSequenceMakerSaveData(float epsilon,
            int minimumCandidates, ActionSaveData lastAction, CandidateSaveData lastOutput,
            Dictionary<string, List<CandidateSaveData>> candidatesDict,
            Dictionary<string, RandomSequenceMakerSaveData> randomMakerDict, int numControlPoints)
        {
            Epsilon = epsilon;
            MinimumCandidates = minimumCandidates;
            LastAction = lastAction;
            LastOutput = lastOutput;
            CandidatesDict = candidatesDict;
            RandomMakerDict = randomMakerDict;
            NumControlPoints = numControlPoints;
        }

        public SimpleBanditSequenceMaker Instantiate()
        {
            return new SimpleBanditSequenceMaker(this);
        }

        ISequenceMaker ISequenceMakerSaveData.Instantiate()
        {
            return Instantiate();
        }
    }
}