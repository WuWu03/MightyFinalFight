using System;
using UnityEngine;

namespace FrameWork
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

        public static void Init()
        {
            if (m_Ins != null)
            {
                Log.Debugger.LogError(string.Format("The instance that Type of {0} has already init", typeof(T).Name));
                return;
            }

            if(m_Manager == null)
            {
                m_Manager = GameObject.Find("GameManager");

                if (m_Manager == null)
                {
                    m_Manager = new GameObject("GameManager");
                    DontDestroyOnLoad(m_Manager);
                }
            }

            m_Ins = m_Manager.GetOrAddComponent<T>();
        }

        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        public virtual void ShutDown()
        {
            m_Ins = null;
        }

        private static T m_Ins = null;
        private static GameObject m_Manager = null;
    }
}