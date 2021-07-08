using System;
using UnityEngine;

namespace GameFrameWork.Event
{
    public class EventMgr : BaseMgr<EventMgr>
    {
        public int EventHandlerCount
        {
            get
            {
                return m_EventPool.EventHandlerCount;
            }
        }

        public int EventCount
        {
            get
            {
                return m_EventPool.EventCount;
            }
        }

        private void Awake()
        {
            m_EventPool = new EventPool<GameEventArgs>();
        }

        private void Update()
        {
            m_EventPool.Update(Time.time, Time.unscaledTime);
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            m_EventPool.Subscribe(id, handler);
        }

        public void UnSubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            m_EventPool.UnSubscibe(id, handler);
        }

        public bool Check(int id, EventHandler<GameEventArgs> handler)
        {
            return m_EventPool.Check(id, handler);
        }

        public int Count(int id)
        {
            return m_EventPool.Count(id);
        }

        public void Dispatch(object sender, GameEventArgs e)
        {
            m_EventPool.Dispatch(sender, e);
        }

        public void DispatchNow(object sender, GameEventArgs e)
        {
            m_EventPool.DispatchNow(sender, e);
        }

        protected override void OnShutDown()
        {
            m_EventPool.ShutDown();
        }

        private EventPool<GameEventArgs> m_EventPool = null;
    }
}