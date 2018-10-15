using System;

namespace MotionGenerator
{
    public class ManipulatableMock
    {
        public virtual int GetManipulatableDimention()
        {
            return 3;
        }
        public virtual Guid GetManipulatableId()
        {
            return Guid.NewGuid();
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