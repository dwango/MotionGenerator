using BodyGenerator.Body.Manipulatables;

namespace MotionGenerator
{
    public class ManipulatableMock : IManipulatable
    {
        public void Init(ITime time, IScanner scanner, ITileScanner tileScanner, IGenealogy genealogy)
        {
        }

        public void SetManipulatableId(int value)
        {
        }

        public virtual int GetManipulatableDimention()
        {
            return 3;
        }

        public int GetManipulatableId()
        {
            return 0;
        }

        public bool IsMoving()
        {
            return false;
        }

        public void Manipulate(MotionSequence sequence)
        {
        }

        public State GetState()
        {
            return new State();
        }

        public void UpdateFixedFrame()
        {
        }

        public void ResetFromPrefab()
        {
        }
    }

    public class ManipulatableMock4Dimention : ManipulatableMock
    {
        public override int GetManipulatableDimention()
        {
            return 4;
        }
    }
}