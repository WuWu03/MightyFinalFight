namespace WuWuFramework.Event
{
    public class EventSender<T> : WuWuFrameworkEventArg, IEventSender where T : struct
    {
        private object m_Sender = null;
        public object sender
        {
            get { return m_Sender; }
        }


        private T m_EventArg = default;
        public T eventArg
        {
            get { return m_EventArg; }
        }

        public static EventSender<T> Create(object sender, T eventArg)
        {
            EventSender<T> eventSender = ReferencePool.Acquire<EventSender<T>>();
            eventSender.m_Sender = sender;
            eventSender.m_EventArg = eventArg;
            return eventSender;
        }

        public override void Clear()
        {
            m_Sender = null;
            m_EventArg = default;
        }

        public void Dispatch(EventPool eventPool)
        {
            eventPool.HandleEvent(this.m_Sender, m_EventArg);
            Release();
        }
    }
}