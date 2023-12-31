using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Event
{
    public class EventPool<T> where T : BaseEventArgs
    {
        public EventPool()
        {
            m_Events = new Queue<Event<T>>();
            m_EventHandlers = new Dictionary<int, List<EventHandler<T>>>();
        }

        public int eventCount
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

        public int Count(int id)
        {
            if (m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                return eventList.Count;
            }

            return 0;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
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

        public void Subscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new Exception("Event handler is invalid.");
            }

            if (!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                eventList = new List<EventHandler<T>>();
                m_EventHandlers.Add(id, eventList);
            }

            if (eventList.Contains(handler))
            {
                throw new Exception("Event handler has already subscribe.");
            }

            eventList.Add(handler);
        }

        public void UnSubscibe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new Exception("Event handler is invalid.");
            }

            if (!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                throw new Exception(StringUtil.Format("Dont't have event ID:", id, "."));
            }

            if (eventList.Contains(handler))
            {
                eventList.Remove(handler);
            }

            if (eventList.Count < 1)
            {
                m_EventHandlers.Remove(id);
            }
        }

        public bool Check(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new Exception("Event handler is invalid.");
            }

            if(!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
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

        public void Clear()
        {
            lock (m_Events)
            {
                m_Events.Clear();
            }
        }

        public void ShutDown()
        {
            Clear();
            m_EventHandlers.Clear();
        }

        private void HandleEvent(object sender, T args)
        {
            int id = args.id;

            if (!m_EventHandlers.TryGetValue(id, out List<EventHandler<T>> eventList))
            {
                return;
            }

            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i]?.Invoke(sender, args);
            }
        }

        private Queue<Event<T>> m_Events = null;
        private Dictionary<int, List<EventHandler<T>>> m_EventHandlers = null;
    }
}
