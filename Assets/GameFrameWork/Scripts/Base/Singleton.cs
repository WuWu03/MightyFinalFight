using System;

namespace GameFrameWork
{
    public abstract class Singleton<T> : IDisposable where T : Singleton<T>, new()
    {
        public static T instance
        {
            get
            {
                m_Instance ??= Activator.CreateInstance<T>();
                return m_Instance;
            }
        }


        public void Dispose()
        {
            m_Instance = null;
            OnDispose();
        }

        protected abstract void OnDispose();

        private static T m_Instance = null;
    }
}