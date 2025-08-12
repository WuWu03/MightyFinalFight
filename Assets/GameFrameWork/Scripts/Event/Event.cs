namespace GameFrameWork.Event
{
    public class Event<T> where T : BaseEventArgs
    {
        public Event()
        {
            m_Sender = null;
            m_EventArgs = null;
        }

        public object sender
        {
            get
            {
                return m_Sender;
            }
        }

        public T eventArgs
        {
            get
            {
                return m_EventArgs;
            }
        }

        public static Event<T> Create(object sender, T eventArgs)
        {
            Event<T> @event = new()
            {
                m_Sender = sender,
                m_EventArgs = eventArgs,
            };
          
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