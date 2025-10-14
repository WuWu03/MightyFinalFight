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

        public void Subscribe(uint eventId, EventHandler<GameEventArg> handler)
        {
            m_EventPool.Subscribe(eventId, handler);
        }

        public void UnSubscribe(uint eventId, EventHandler<GameEventArg> handler)
        {            
            m_EventPool.UnSubscibe(eventId, handler);
        }

        public bool Check(uint eventId, EventHandler<GameEventArg> handler)
        {
            return m_EventPool.Check(eventId, handler);
        }

        public int Count(uint eventId)
        {
            return m_EventPool.Count(eventId);
        }

        public void Dispatch(object sender, GameEventArg arg)
        {
            m_EventPool.Dispatch(sender, arg);
        }

        public void DispatchNow(object sender, GameEventArg arg)
        {
            m_EventPool.DispatchNow(sender, arg);
        }

        private EventPool<GameEventArg> m_EventPool = null;
    }
}