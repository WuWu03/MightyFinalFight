using GameFrameWork.Event;
using UnityEngine;

namespace GameFrameWork
{
    public class MonoBehaviourMgr : BaseMgr<MonoBehaviourMgr>
    {
        private event GameFrameWorkAction<float, float, float, float> m_UpdateEvent;
        private event GameFrameWorkAction<float, float, float, float> m_LateUpdateEvent;
        private event GameFrameWorkAction<float, float, float, float> m_FixedUpdateEvent;
        public event GameFrameWorkAction<float, float, float, float> updateEvent
        {
            add
            {
                m_UpdateEvent += value;
            }
            remove
            {
                m_UpdateEvent -= value;
            }
        }

        public event GameFrameWorkAction<float, float, float, float> lateUpdateEvent
        {
            add
            {
                m_LateUpdateEvent += value;
            }
            remove
            {
                m_LateUpdateEvent -= value;
            }
        }
        
        public event GameFrameWorkAction<float, float, float, float> fixedUpdateEvent
        {
            add
            {
                m_FixedUpdateEvent += value;
            }
            remove
            {
                m_FixedUpdateEvent -= value;
            }
        }

        private void Update()
        {
            m_UpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }

        private void LateUpdate()
        {
            m_LateUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime, Time.time, Time.unscaledTime);
        }

        private void FixedUpdate()
        {
            m_FixedUpdateEvent?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime, Time.fixedTime, Time.fixedUnscaledTime);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            m_UpdateEvent = null;
            m_LateUpdateEvent = null;
            m_FixedUpdateEvent = null;
        }
    }
}