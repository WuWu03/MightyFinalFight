using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class BaseMgr<T> : MonoBehaviour where T : BaseMgr<T>, new()
    {
        public static T instance
        {
            get
            {
                if (s_Instance == null)
                {
                    Log.LogError(StringUtil.Append(typeof(T).Name, "没有实例，请先初始化该实例"));
                    return null;
                }

                return s_Instance;
            }
        }

        public static void Init(GameObject manager)
        {
            if (s_Instance != null)
            {
                Log.LogError(StringUtil.Append(typeof(T).Name), "实例已经存在，请不要重复实例化");
                return;
            }

            if (manager == null)
            {
                Log.LogError("管理器为空");
                return;
            }

            s_Instance = manager.GetOrAddComponent<T>();
            s_Instance.StartUp();
        }

        public void StartUp()
        {
            s_Instance.m_Running = true;
        }

        public void ShutDown()
        {
            m_Running = false;
            OnShutDown();
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
        }

        private void LateUpdate()
        {
            if (m_Running)
            {
                OnLateUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (m_Running)
            {
                OnFixedUpdate();
            }
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnShutDown() { }
        protected virtual void OnDestory() { }

        private void OnDestroy()
        {
            if (!m_Running)
            {
                return;
            }

            ShutDown();
            OnDestory();
        }

        private bool m_Running = false;
        private static T s_Instance = null;
    }
}