using WuWuFramework.Utils;
using UnityEngine;

namespace WuWuFramework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T s_Instance;
        public static T instance
        {
            get
            {
                if (s_Instance == null)
                {

                    T[] instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);

                    if (instances != null && instances.Length > 0)
                    {
                        if (instances.Length > 1)
                        {
                            throw new WuWuFrameworkException(StringUtil.Append(typeof(T).Name, "实例超过一个 , 请不要重复实例化"));
                        }
                        
                        s_Instance = instances[0];
                    }
                    else
                    {
                        s_Instance = new GameObject(typeof(T).Name).GetOrAddComponent<T>();

                        if(Application.isPlaying)
                        {
                            DontDestroyOnLoad(s_Instance.gameObject);
                        }
                    }
                }

                return s_Instance;
            }
        }
    }
}