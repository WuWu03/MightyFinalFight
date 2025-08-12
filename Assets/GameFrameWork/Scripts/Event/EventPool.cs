using System;
using System.Collections.Generic;

namespace GameFrameWork.Event
{
    public class EventPool<T> where T : BaseEventArgs
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
            m_Events = new Queue<Event<T>>();
            m_EventHandlers = new Dictionary<int, List<EventHandler<T>>>();
        }


        public int Count(int id)
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
                    Event<T> @event = m_Events.Dequeue();
                    HandleEvent(@event.sender, @event.eventArgs);
                }
            }
        }

        public void Subscribe(int eventId, EventHandler<T> handler)
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

        public void UnSubscibe(int eventId, EventHandler<T> handler)
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

        public bool Check(int eventId, EventHandler<T> handler)
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

        public void Dispatch(object sender, T args)
        {
            Event<T> item = Event<T>.Create(sender, args);

            lock (m_Events)
            {
                m_Events.Enqueue(item);
            }
        }

        public void DispatchNow(object sender, T args)
        {
            HandleEvent(sender, args);
        }

        public void ShutDown()
        {
            m_Events.Clear();
            m_EventHandlers.Clear();
        }

        private void HandleEvent(object sender, T args)
        {
            int id = args.id;

            if (!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                return;
            }

            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                eventList[i]?.Invoke(sender, args);
            }

            args.Release();
        }

        private readonly Queue<Event<T>> m_Events = null;
        private readonly Dictionary<int, List<EventHandler<T>>> m_EventHandlers = null;
    }
}