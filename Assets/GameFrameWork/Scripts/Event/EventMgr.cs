using System;

namespace GameFrameWork.Event
{
    public class EventMgr : GameFrameWorkModule , IEventMgr
    {
        private readonly EventPool<EventArg> m_EventPool;
        public EventMgr()
        {
            m_EventPool = new();
        }
        
        public int currEventCount
        {
            get
            {
                return m_EventPool.currEventCount;
            }
        }

        public int eventHandlerCount
        {
            get
            {
                return m_EventPool.eventHandlerCount;
            }
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            m_EventPool.Update();
        }

        public override void Shutdown()
        {
            m_EventPool.ShutDown();
        }
        
        public void Subscribe(uint eventId, EventHandler<EventArg> handler)
        {
            m_EventPool.Subscribe(eventId, handler);
        }

        public void UnSubscribe(uint eventId, EventHandler<EventArg> handler)
        {            
            m_EventPool.UnSubscibe(eventId, handler);
        }

        public bool Check(uint eventId, EventHandler<EventArg> handler)
        {
            return m_EventPool.Check(eventId, handler);
        }

        public int Count(uint eventId)
        {
            return m_EventPool.Count(eventId);
        }

        public void Dispatch(object sender, EventArg arg)
        {
            m_EventPool.Dispatch(sender, arg);
        }

        public void DispatchNow(object sender, EventArg arg)
        {
            m_EventPool.DispatchNow(sender, arg);
        }
    }
}