using System;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class BaseMgr<T>: MonoBehaviour where T:BaseMgr<T>,new()
    {
        public static T Ins
        {
            get
            {
                if (m_Ins == null)
                {
                    Log.Debugger.LogError(string.Format("The instance that Type of {0} must be init", typeof(T).Name));
                    return null;
                }

                return m_Ins;
            }
        }

        public static void Init(GameObject manager)
        {
            if (m_Ins != null)
            {
                Log.Debugger.LogError(string.Format("The instance that Type of {0} has already init", typeof(T).Name));
                return;
            }

            if(manager == null)
            {
                Log.Debugger.LogError("The manager is missing");
                return;
            }

            m_Ins = manager.GetOrAddComponent<T>();
            m_Ins.m_Running = true;
        }

        public virtual int Priority
        {
            get
            {
                return 0;
            }
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
            ShutDown();
        }

        private bool m_Running = false;
        private static T m_Ins = null;
    }
}