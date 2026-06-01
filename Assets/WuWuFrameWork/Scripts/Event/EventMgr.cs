using System;

namespace WuWuFramework.Event
{
    public class EventMgr : WuWuFrameworkModule, IEventMgr
    {
        private readonly EventPool m_EventPool;
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

        public UnSubscribe Subscribe<T>(EventHandler<T> handler) where T : struct
        {
            return m_EventPool.Subscribe(handler);
        }

        public void UnSubscribe<T>(EventHandler<T> handler) where T : struct
        {
            m_EventPool.UnSubscibe(handler);
        }

        public bool Check<T>(EventHandler<T> handler) where T : struct
        {
            return m_EventPool.Check(handler);
        }

        public int Count<T>() where T : struct
        {
            return m_EventPool.Count<T>();
        }

        public void Dispatch<T>(object sender, T arg) where T : struct
        {
            m_EventPool.Dispatch(sender, arg);
        }

        public void DispatchNow<T>(object sender, T arg) where T : struct
        {
            m_EventPool.DispatchNow(sender, arg);
        }
    }
}