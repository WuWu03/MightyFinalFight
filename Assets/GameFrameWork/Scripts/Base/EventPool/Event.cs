namespace GameFrameWork
{
    public class Event<T> where T : BaseEventArgs
    {
        public Event()
        {
            m_Sender = null;
            m_EventArgs = null;
        }

        public object Sender
        {
            get
            {
                return m_Sender;
            }
        }

        public T EventArgs
        {
            get
            {
                return m_EventArgs;
            }
        }

        public static Event<T> Create(object sender, T eventArgs)
        {
            Event<T> @event = new Event<T>();
            @event.m_Sender = sender;
            @event.m_EventArgs = eventArgs;
            return @event;
        }

        public void Clear()
        {
            m_Sender = null;
            m_EventArgs = null;
        }

        private object m_Sender = null;
        private T m_EventArgs = null;
    }
}