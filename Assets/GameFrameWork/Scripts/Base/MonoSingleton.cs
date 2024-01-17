using UnityEngine;

namespace GameFrameWork
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {

                    T[] instances = GameObject.FindObjectsOfType<T>();

                    if (instances != null && instances.Length > 0)
                    {
                        if (instances.Length > 1)
                        {
                            Log.LogError("The instance that Type of ", typeof(T).Name, " is more than one");
                            return null;
                        }
                        else
                        {
                            m_Instance = instances[0];
                        }
                    }
                    else
                    {
                        m_Instance = new GameObject(typeof(T).Name).GetOrAddComponent<T>();

                        if(Application.isPlaying)
                        {
                            DontDestroyOnLoad(m_Instance.gameObject);
                        }
                    }
                }

                return m_Instance;
            }
        }

        protected static T m_Instance;
    }
}