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
    }


    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class BaseSingleton
    {
        protected static readonly List<BaseSingleton> s_Objects = new();

        public abstract void Shutdown();

        /// <summary>
        /// 释放所有单例对象
        /// </summary>
        public static void ShutdownAll()
        {
            for (int i = 0; i < s_Objects.Count; i++)
            {
                s_Objects[i].Shutdown();
            }

            s_Objects.Clear();
        }
    }
}