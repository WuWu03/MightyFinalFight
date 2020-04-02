using UnityEngine;

namespace FrameWork
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Ins
        {
            get
            {
                if (m_Ins == null)
                {
                    if (!m_IsAutoInstantiate)
                    {
                        Logger.LogError(string.Format("The instance that Type of {0} must be init", typeof(T).Name));
                        return null;
                    }

                    T[] instance = GameObject.FindObjectsOfType<T>();

                    if (instance != null && instance.Length > 0)
                    {
                        if (instance.Length > 1)
                        {
                            Logger.LogError(string.Format("The instance that Type of {0} is more than one", typeof(T).Name));
                            return null;
                        }
                        else
                        {
                            m_Ins = instance[0];
                        }
                    }
                    else
                    {
                        GameObject instanceObj = GameObject.Find(typeof(T).Name);
                        if (instanceObj == null) instanceObj = new GameObject(typeof(T).Name);
                        m_Ins = instanceObj.GetOrAddComponent<T>();
                    }
                }
                return m_Ins;
            }
        }

        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }

        protected static T m_Ins;
        protected static bool m_IsAutoInstantiate = true;
    }
}