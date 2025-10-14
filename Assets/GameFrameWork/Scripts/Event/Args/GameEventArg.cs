namespace GameFrameWork.Event
{
    public abstract class GameEventArg : BaseEventArg
    {

    }
    
    public abstract class GameEventArg<T> : GameEventArg where T : GameEventArg<T>, new()
    {
        private uint m_Id;

        public override uint id
        {
            get { return m_Id; }
        }

        public static T Create(uint eventId)
        {
            T arg = ReferencePool.Acquire<T>();
            arg.m_Id = eventId;
            return arg;
        }

        public override void Clear()
        {
            m_Id = 0;
        }
    }
}