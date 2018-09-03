using MotionGenerator.Serialization;

namespace MotionGenerator.Entity.Soul
{
    public class GluttonySoul : Soul
    {
        public const string Key = "eatenFoods";

        public override float Reward(State lastState, State nowState)
        {
            if (lastState.ContainsKey(Key) && nowState.ContainsKey(Key))
            {
                var gottenEnergy = (float) nowState[Key][0] - (float) lastState[Key][0];
                if (gottenEnergy > 0f)
                {
                    gottenEnergy = 1f;
                }

                return gottenEnergy - 0.01f;
            }
            else
            {
                return 0;
            }
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }

    public class EnergizeSoul : Soul
    {
        private const string Key = State.BasicKeys.OrganEnergy;

        public override float Reward(State lastState, State nowState)
        {
            if (lastState.ContainsKey(Key) && nowState.ContainsKey(Key))
            {
                var gottenEnergy = (float) nowState[Key][0] - (float) lastState[Key][0];
                return gottenEnergy * 100000; // for paper ...
            }
            else
            {
                return 0;
            }
        }

        public override SoulSaveData SaveAsInterface()
        {
            return new SoulSaveData(GetType(), MotionGeneratorSerialization.Serialize(Save()));
        }
    }
}