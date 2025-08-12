using System;

namespace GameFrameWork.Event
{
    public class EventMgr : BaseMgr<EventMgr>
    {
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

        protected override void OnAwake()
        {
            base.OnAwake();
            m_EventPool = new();
        }

        protected override void OnUpdate()
        {
            m_EventPool.Update();
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            m_EventPool.ShutDown();
        }

        protected override void OnDestory()
        {
            base.OnDestory();
            m_EventPool = null;
        }

        public void Subscribe(int eventId, EventHandler<GameEventArgs> handler)
        {
            m_EventPool.Subscribe(eventId, handler);
        }

        public void UnSubscribe(int eventId, EventHandler<GameEventArgs> handler)
        {            
            m_EventPool.UnSubscibe(eventId, handler);
        }

        public bool Check(int eventId, EventHandler<GameEventArgs> handler)
        {
            return m_EventPool.Check(eventId, handler);
        }

        public int Count(int eventId)
        {
            return m_EventPool.Count(eventId);
        }

        public void Dispatch(object sender, GameEventArgs e)
        {
            m_EventPool.Dispatch(sender, e);
        }

        public void DispatchNow(object sender, GameEventArgs e)
        {
            m_EventPool.DispatchNow(sender, e);
        }

        private EventPool<GameEventArgs> m_EventPool = null;
    }
}