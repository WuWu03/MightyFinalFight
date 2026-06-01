using System;
using System.Collections.Generic;
using UnityEngine;

namespace WuWuFramework.Event
{
    public abstract class UnSubscribeTrigger : MonoBehaviour
    {
        private HashSet<Type> m_UnSubscribeEvents = new();
        private EventPool m_EventPool = null;

        public bool AddUnSubscribeEvent(Type eventType)
        {
            return m_UnSubscribeEvents.Add(eventType);
        }

        public void SetEventPool(EventPool eventPool)
        {
            m_EventPool = eventPool;
        }

        protected void UnSubscribeAll()
        {
            if (m_EventPool == null)
            {
                return;
            }

            foreach (Type eventType in m_UnSubscribeEvents)
            {
                m_EventPool.UnSubscibeAll(eventType);
            }

            m_EventPool = null;
        }
    }
}
