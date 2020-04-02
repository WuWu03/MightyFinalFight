using System;
using UnityEngine;
using System.Collections;

namespace FrameWork
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        public static T Ins
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Activator.CreateInstance<T>();
                }
                return m_Instance;
            }
        }

        protected static T m_Instance = null;
    }
}