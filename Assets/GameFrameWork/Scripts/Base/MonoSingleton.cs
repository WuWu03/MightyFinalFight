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

                    T[] instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);

                    if (instances != null && instances.Length > 0)
                    {
                        if (instances.Length > 1)
                        {
                            Log.LogError(typeof(T).Name, "实例超过一个 , 请不要重复实例化");
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