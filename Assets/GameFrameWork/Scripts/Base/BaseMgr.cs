using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class BaseMgr<T> : MonoBehaviour where T : BaseMgr<T>, new()
    {
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    Log.LogError(StringUtil.Format("The instance that Type of ", typeof(T).Name, " must be init"));
                    return null;
                }

                return m_Instance;
            }
        }

        public static void Init(GameObject manager)
        {
            if (m_Instance != null)
            {
                Log.LogError(StringUtil.Format("The instance that Type of ", typeof(T).Name), " has already init");
                return;
            }

            if (manager == null)
            {
                Log.LogError("The manager is missing");
                return;
            }

            m_Instance = manager.GetOrAddComponent<T>();
            m_Instance.m_Running = true;
        }

        public virtual void Run()
        {
            m_Running = true;
            OnRun();
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
                OnUpdate();
        }

        private void LateUpdate()
        {
            if (m_Running)
                OnLateUpdate();
        }

        protected virtual void OnRun() { }

        protected virtual void OnShutDown() { }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnLateUpdate() { }

        private void OnDestroy()
        {
            if (!m_Running)
            {
                return;
            }

            ShutDown();
        }

        private bool m_Running = false;
        private static T m_Instance = null;
    }
}