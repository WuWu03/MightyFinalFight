using System;
using UnityEngine;

namespace WuWuFramework.Event
{
    public class UnSubscribe
    {
        private EventPool m_EventPool = null;
        private Type m_CurrEventType = null;

        public UnSubscribe(EventPool eventPool)
        {
            m_EventPool = eventPool;
        }

        public void SetCurrEventType(Type eventType)
        {
            m_CurrEventType = eventType;
        }

        public void UnSubscribeAllOnDestroy(GameObject go)
        {
            if (go is null)
            {
                return;
            }

            UnSubscribeTrigger unSubscribeTrigger = go.GetOrAddComponent<UnSubscribeTriggerOnDestroy>();
            unSubscribeTrigger.AddUnSubscribeEvent(m_CurrEventType);
            unSubscribeTrigger.SetEventPool(m_EventPool);
        }

        public void UnSubscribeAllOnDisable(GameObject go)
        {
            if (go is null)
            {
                return;
            }

            UnSubscribeTrigger unSubscribeTrigger = go.GetOrAddComponent<UnSubscribeTriggerOnDisable>();
            unSubscribeTrigger.AddUnSubscribeEvent(m_CurrEventType);
            unSubscribeTrigger.SetEventPool(m_EventPool);
        }
    }
}