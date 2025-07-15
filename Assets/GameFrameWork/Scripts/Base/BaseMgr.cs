using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class BaseMgr<T> : MonoBehaviour where T : BaseMgr<T>, new()
    {
        public event GameFrameWorkAction<float,float> updateEvent
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

        public event GameFrameWorkAction<float, float> lateUpdateEvent
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

        public event GameFrameWorkAction<float, float> fixedUpdateEvent
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

        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    Log.LogError(StringUtil.Append(typeof(T).Name, "没有实例，请先初始化该实例"));
                    return null;
                }

                return m_Instance;
            }
        }

        public static void Init(GameObject manager)
        {
            if (m_Instance != null)
            {
                Log.LogError(StringUtil.Append(typeof(T).Name), "实例已经存在，请不要重复实例化");
                return;
            }

            if (manager == null)
            {
                Log.LogError("管理器为空");
                return;
            }

            m_Instance = manager.GetOrAddComponent<T>();
            m_Instance.m_Running = true;
        }

        public void ShutDown()
        {
            m_Running = false;
            OnShutDown();
            DestroyImmediate(m_Instance);
        }

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            if (m_Running)
            {
                OnUpdate();
            }

            m_UpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            if (m_Running)
            {
                OnLateUpdate();
            }

            m_LateUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
            if (m_Running)
            {
                OnFixedUpdate();
            }

            m_FixedUpdateEvent?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
        }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnLateUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnShutDown() { }

        private void OnDestroy()
        {
            if (!m_Running)
            {
                return;
            }

            ShutDown();
        }

        private event GameFrameWorkAction<float, float> m_UpdateEvent = null;
        private event GameFrameWorkAction<float, float> m_LateUpdateEvent = null;
        private event GameFrameWorkAction<float, float> m_FixedUpdateEvent = null;

        private bool m_Running = false;
        private static T m_Instance = null;
    }
}