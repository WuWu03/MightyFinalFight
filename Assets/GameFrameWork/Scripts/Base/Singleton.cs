using System;
using UnityEngine;
using System.Collections;

namespace GameFrameWork
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance
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