using System.Collections.Generic;

namespace WuWuFramework
{
    /// <summary>
    /// 单例模板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : BaseSingleton where T : Singleton<T>, new()
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static T m_Instance = null;

        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                    s_Objects.Add(m_Instance);
                }

                return m_Instance;
            }
        }

        public override void ShutDown()
        {
            OnShutdown();
        }

        protected abstract void OnShutdown();
    }

    public abstract class BaseSingleton
    {
        protected static readonly List<BaseSingleton> s_Objects = new();

        public abstract void ShutDown();

        public static void ShutDownAll()
        {
            for (int i = 0; i < s_Objects.Count; i++)
            {
                s_Objects[i].ShutDown();
            }

            s_Objects.Clear();
        }
    }
}