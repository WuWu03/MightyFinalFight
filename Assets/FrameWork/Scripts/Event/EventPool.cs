using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Event
{
    public class EventPool<T> where T : BaseEventArgs
    {
        public EventPool()
        {
            m_Events = new Queue<Event>();
            m_EventHandlers = new Dictionary<int, List<EventHandler<T>>>();
        }

        public int EventCount
        {
            get
            {
                return m_Events.Count;
            }
        }

        public int EventHandlerCount
        {
            get
            {
                return m_EventHandlers.Count;
            }
        }

        public int Count(int id)
        {
            List<EventHandler<T>> eventList = null;

            if (m_EventHandlers.TryGetValue(id, out eventList))
            {
                return eventList.Count;
            }

            return 0;
        }

        public void Update()//float elapseSeconds, float realElapseSeconds)
        {
            while (this.m_Events.Count > 0)
            {
                EventPool<T>.Event @event = null;
                Queue<Event> events = m_Events;

                lock (events)
                {
                    @event = m_Events.Dequeue();
                    HandleEvent(@event.Sender, @event.EventArgs);
                }
            }
        }

        public void Subscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new Exception("Event handler is invalid.");
            }

            List<EventHandler<T>> eventList = null;

            if (!m_EventHandlers.TryGetValue(id, out eventList))
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

            List<EventHandler<T>> eventList = null;

            if (!m_EventHandlers.TryGetValue(id, out eventList))
            {
                throw new Exception(string.Format("Dont't have event ID:{0}.", id));
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

            List<EventHandler<T>> eventList = null;

            if(!m_EventHandlers.TryGetValue(id,out eventList))
            {
                return false;
            }

            return eventList.Contains(handler);
        }

        public void Dispatch(object sender, T args)
        {
            Event item = Event.Create(sender, args);
            Queue<EventPool<T>.Event> events = this.m_Events;
            lock (events)
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
            Queue<Event> events = m_Events;

            lock (events)
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
            int id = args.ID;
            List<EventHandler<T>> eventList = null;

            if (!m_EventHandlers.TryGetValue(id, out eventList))
            {
                throw new Exception(string.Format("Dont't have event ID:{0}.", id));
            }

            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i]?.Invoke(sender, args);
            }
        }

        private Queue<Event> m_Events = null;
        private Dictionary<int, List<EventHandler<T>>> m_EventHandlers = null;
        class Event
        {
            public Event()
            {
                m_Sender = null;
                m_EventArgs = null;
            }

            public object Sender
            {
                get
                {
                    return m_Sender;
                }
            }

            public T EventArgs
            {
                get
                {
                    return m_EventArgs;
                }
            }

            public static Event Create(object sender, T eventArgs)
            {
                Event @event = new Event();
                @event.m_Sender = sender;
                @event.m_EventArgs = eventArgs;
                return @event;
            }

            public void Clear()
            {
                m_Sender = null;
                m_EventArgs = null;
            }

            private object m_Sender = null;
            private T m_EventArgs = null;
        }
    }
}
