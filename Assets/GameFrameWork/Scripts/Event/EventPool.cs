using System;
using System.Collections.Generic;

namespace GameFrameWork.Event
{
    public class EventPool<T> where T : GameEventArg
    {
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

        public EventPool()
        {
            m_Events = new();
            m_EventHandlers = new();
        }
        
        public int Count(uint id)
        {
            if (m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
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
                    EventSender<T> eventSender = m_Events.Dequeue();
                    HandleEvent(eventSender.sender, eventSender.eventArg);
                    eventSender.Release();
                }
            }
        }

        public void Subscribe(uint eventId, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Log.LogError("事件 [", eventId.ToString(), "] 的回调函数为空");
                return;
            }

            if (!m_EventHandlers.TryGetValue(eventId, out List<EventHandler<T>> eventList))
            {
                eventList = new List<EventHandler<T>>();
                m_EventHandlers.Add(eventId, eventList);
            }

            if (eventList.Contains(handler))
            {
                Log.LogError("事件 [", eventId.ToString(), "] 重复订阅");
                return;
            }

            eventList.Add(handler);
        }

        public void UnSubscibe(uint eventId, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Log.LogError("事件 [", eventId.ToString(), "] 的回调函数为空");
                return;
            }

            if (!m_EventHandlers.TryGetValue(eventId, out List<EventHandler<T>> eventList))
            {
                Log.LogError("事件 [", eventId.ToString(), "] 不存在");
                return;
            }

            if (eventList.Contains(handler))
            {
                eventList.Remove(handler);
            }

            if (eventList.Count < 1)
            {
                m_EventHandlers.Remove(eventId);
            }
        }

        public bool Check(uint eventId, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Log.LogError("事件 [", eventId.ToString(), "] 的回调函数为空");
            }

            if (!m_EventHandlers.TryGetValue(eventId, out List<EventHandler<T>> eventList))
            {
                return false;
            }

            return eventList.Contains(handler);
        }

        public void Dispatch(object sender, T arg)
        {
            EventSender<T> eventSender = EventSender<T>.Create(sender, arg);

            lock (m_Events)
            {
                m_Events.Enqueue(eventSender);
            }
        }

        public void DispatchNow(object sender, T arg)
        {
            HandleEvent(sender, arg);
        }

        public void ShutDown()
        {
            m_Events.Clear();
            m_EventHandlers.Clear();
        }

        private void HandleEvent(object sender, T arg)
        {
            uint id = arg.id;

            if (!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                return;
            }

            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                eventList[i]?.Invoke(sender, arg);
            }

            arg.Release();
        }

        private readonly Queue<EventSender<T>> m_Events = null;
        private readonly Dictionary<uint, List<EventHandler<T>>> m_EventHandlers = null;
    }
}