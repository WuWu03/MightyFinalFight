using System;
using UnityEngine;

namespace FrameWork
{
    public abstract class BaseMgr<T>:MonoSingleton<T> where T:BaseMgr<T>,new()
    {
        public static void Init()
        {
            if (m_Ins != null)
            {
                Logger.LogError(string.Format("The instance that Type of {0} must be init", typeof(T).Name));
                return;
            }

            GameObject go = GameObject.Find("GameManager");

            if (go == null)
            {
                go = new GameObject("GameManager");
                DontDestroyOnLoad(go);
            }

            m_Ins = go.GetOrAddComponent<T>();
        }

        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        public abstract void ShutDown();
        protected new static bool m_IsAutoInstantiate = false;
    }
}