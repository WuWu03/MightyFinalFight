using System;

namespace WuWuFramework.Event
{
    public class EventMgr : WuWuFrameworkModule, IEventMgr
    {
        private readonly EventPool m_EventPool;

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

        public EventMgr()
        {
            m_EventPool = new();
            MonoBehaviourMgr.instance.updateEvent += Update;
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public UnSubscribe Subscribe<T>(EventHandler<T> handler) where T : struct
        {
            return m_EventPool.Subscribe(handler);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void UnSubscribe<T>(EventHandler<T> handler) where T : struct
        {
            m_EventPool.UnSubscibe(handler);
        }

        /// <summary>
        /// 检测是否
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
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

        public override void Shutdown()
        {
            MonoBehaviourMgr.instance.updateEvent -= Update;
            m_EventPool.ShutDown();
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            m_EventPool.Update();
        }
    }
}