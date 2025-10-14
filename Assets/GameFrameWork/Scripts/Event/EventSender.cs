namespace GameFrameWork.Event
{
    public class EventSender<T> : GameFrameWorkEventArg where T : GameEventArg
    {
        private object m_Sender = null;
        public object sender
        {
            get { return m_Sender; }
        }

        
        private T m_EventArg = null;
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
            m_EventArg = null;
        }
    }
}