using System;
using System.Collections.Generic;

namespace GameFrameWork.Event
{
    public class EventPool
    {
        private readonly Queue<IEventSender> m_Events;
        private readonly Dictionary<Type, List<object>> m_EventHandlers;
        private readonly UnSubscribe m_UnSubscribe;

        public EventPool()
        {
            m_Events = new();
            m_EventHandlers = new();
            m_UnSubscribe = new(this);
        }

        public int currEventCount
        {
            get
            {
                return m_Events.Count;
            }
        }

        public int eventHandlerCount
        {
            get
            {
                return m_EventHandlers.Count;
            }
        }

        public int Count<T>() where T : struct
        {
            if (m_EventHandlers.TryGetValue(typeof(T), out List<object> eventList))
            {
                return eventList.Count;
            }

            return 0;
        }

        public void Update()
        {
            while (m_Events.Count > 0)
            {
                lock (m_Events)
                {
                    IEventSender eventSender = m_Events.Dequeue();
                    eventSender.Dispatch(this);
                }
            }
        }

        public UnSubscribe Subscribe<T>(EventHandler<T> handler) where T : struct
        {
            Type eventType = typeof(T);

            if (handler == null)
            {
                Log.LogError("事件 [", eventType.Name, "] 的回调函数为空");
                return null;
            }

            if (!m_EventHandlers.TryGetValue(eventType, out List<object> eventList))
            {
                eventList = new();
                m_EventHandlers.Add(eventType, eventList);
            }

            if (eventList.Contains(handler))
            {
                Log.LogError("事件 [", eventType.Name, "] 重复订阅");
                return null;
            }

            eventList.Add(handler);
            m_UnSubscribe.SetCurrEventType(eventType);
            return m_UnSubscribe;
        }

        public void UnSubscibe<T>(EventHandler<T> handler) where T : struct
        {
            Type eventType = typeof(T);

            if (handler == null)
            {
                Log.LogError("事件 [", eventType.Name, "] 的回调函数为空");
                return;
            }

            if (!m_EventHandlers.TryGetValue(eventType, out List<object> eventList))
            {
                Log.LogError("事件 [", eventType.Name, "] 不存在");
                return;
            }

            if (eventList.Contains(handler))
            {
                eventList.Remove(handler);
            }

            if (eventList.Count < 1)
            {
                m_EventHandlers.Remove(eventType);
            }
        }

        public void UnSubscibeAll(Type eventType)
        {
            if (!m_EventHandlers.TryGetValue(eventType, out List<object> eventList))
            {
                Log.LogError("事件 [", eventType.Name, "] 不存在");
                return;
            }
            m_EventHandlers.Remove(eventType);
        }

        public bool Check<T>(EventHandler<T> handler) where T : struct
        {
            Type eventType = typeof(T);

            if (handler == null)
            {
                Log.LogError("事件 [", eventType.Name, "] 的回调函数为空");
            }

            if (!m_EventHandlers.TryGetValue(eventType, out List<object> eventList))
            {
                return false;
            }

            return eventList.Contains(handler);
        }

        public void Dispatch<T>(object sender, T arg) where T : struct
        {
            EventSender<T> eventSender = EventSender<T>.Create(sender, arg);

            lock (m_Events)
            {
                m_Events.Enqueue(eventSender);
            }
        }

        public void DispatchNow<T>(object sender, T arg) where T : struct
        {
            HandleEvent(sender, arg);
        }

        public void ShutDown()
        {
            m_Events.Clear();
            m_EventHandlers.Clear();
        }

        public void HandleEvent<T>(object sender, T arg) where T : struct
        {
            Type eventType = typeof(T);

            if (!m_EventHandlers.TryGetValue(eventType, out List<object> eventList))
            {
                return;
            }

            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                (eventList[i] as EventHandler<T>)?.Invoke(sender, arg);
            }
        }
    }
}