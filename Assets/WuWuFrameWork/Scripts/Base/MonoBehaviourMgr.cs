using UnityEngine;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework
{
    public class MonoBehaviourMgr : MonoBehaviour
    {
        private static MonoBehaviourMgr s_Instance;
        private event WuWuFrameworkAction<float, float, float, float> m_UpdateEvent;
        private event WuWuFrameworkAction<float, float, float, float> m_LateUpdateEvent;
        private event WuWuFrameworkAction<float, float, float, float> m_FixedUpdateEvent;

        public static MonoBehaviourMgr instance
        {
            get
            {
                if (s_Instance is null)
                {
                    throw new WuWuFrameworkException(StringUtil.Append("[MonoBehaviourMgr] 没有实例，请先初始化该实例"));
                }

                return s_Instance;
            }
        }

        public event WuWuFrameworkAction<float, float, float, float> updateEvent
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

        public event WuWuFrameworkAction<float, float, float, float> lateUpdateEvent
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
        
        public event WuWuFrameworkAction<float, float, float, float> fixedUpdateEvent
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

        public static void Init(GameObject manager)
        {
            if (s_Instance is not null)
            {
                throw new WuWuFrameworkException(StringUtil.Append("[MonoBehaviourMgr] 实例已经存在，请不要重复实例化"));
            }

            if (manager is null)
            {
                throw new WuWuFrameworkException("管理器为空");
            }

            s_Instance = manager.GetOrAddComponent<MonoBehaviourMgr>();
        }

        public void ShutDown()
        {
            m_UpdateEvent = null;
            m_LateUpdateEvent = null;
            m_FixedUpdateEvent = null;
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

        private void OnDestroy()
        {
            ShutDown();
        }
    }
}