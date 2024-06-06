using System;

namespace GameFrameWork.Event
{
    public struct GameFrameWorkCommonEvent
    {
        public const int LanguageChangeEvent = -9998;
        public const int ApplicationQuitEvent = -9999;
    }

    public class EventMgr : BaseMgr<EventMgr>
    {
        public int eventHandlerCount
        {
            get
            {
                return m_EventPool.eventHandlerCount;
            }
        }

        public int eventCount
        {
            get
            {
                return m_EventPool.eventCount;
            }
        }

        private void Awake()
        {
            m_EventPool = new EventPool<GameEventArgs>();
        }

        private void Update()
        {
            m_EventPool.Update();
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
            base.OnShutDown();
            m_EventPool.ShutDown();
            m_EventPool = null;
        }

        private EventPool<GameEventArgs> m_EventPool = null;
    }
}