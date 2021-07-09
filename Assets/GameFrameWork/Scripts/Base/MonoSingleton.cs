using GameFrameWork.Utility;
using GameFrameWork.Log;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Ins
        {
            get
            {
                if (m_Ins == null)
                {

                    T[] instances = GameObject.FindObjectsOfType<T>();

                    if (instances != null && instances.Length > 0)
                    {
                        if (instances.Length > 1)
                        {
                            GameFrameworkLog.LogError("The instance that Type of ", typeof(T).Name, " is more than one");
                            return null;
                        }
                        else
                        {
                            m_Ins = instances[0];
                        }
                    }
                    else
                    {
                        m_Ins = new GameObject(typeof(T).Name).GetOrAddComponent<T>();
                        DontDestroyOnLoad(m_Ins.gameObject);
                    }
                }
                return m_Ins;
            }
        }

        protected static T m_Ins;
    }
}