using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    [Serializable]
    public class UIEvent<T> where T : BaseEventData
    {
        public UIEvent()
        {
            m_UIEventArgs = new();
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, byte> handle, byte arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, short> handle, short arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, int> handle, int arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, long> handle, long arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, float> handle, float arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, double> handle, double arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void AddListener(GameFrameWorkAction<GameObject, T, bool> handle, bool arg)
        {
            if (HasCall(handle))
            {
                Log.LogError("事件已经存在");
                return;
            }

            UIEventArg<T> uiEventArg = UIEventArg<T>.Create(handle, arg);
            m_UIEventArgs.Add(uiEventArg);
        }

        public void RemoveListener(GameFrameWorkAction<GameObject, T, int> handle)
        {
            RemoveCall(handle);
        }

        public void RemoveListener(GameFrameWorkAction<GameObject, T, long> handle)
        {
            RemoveCall(handle);
        }

        public void RemoveListener(GameFrameWorkAction<GameObject, T, float> handle)
        {
            RemoveCall(handle);
        }

        public void RemoveListener(GameFrameWorkAction<GameObject, T, double> handle)
        {
            RemoveCall(handle);
        }

        public void RemoveListener(GameFrameWorkAction<GameObject, T, bool> handle)
        {
            RemoveCall(handle);
        }

        public void RemoveAllListeners()
        {
            m_UIEventArgs.Clear();
        }

        public void Invoke(GameObject go, T eventData)
        {
            foreach (UIEventArg<T> arg in m_UIEventArgs)
            {
                if (arg.argType == typeof(int))
                {
                    GameFrameWorkAction<GameObject, T, int> call = arg.call as GameFrameWorkAction<GameObject, T, int>;
                    call.Invoke(go, eventData, arg.intArg);
                }
                else if(arg.argType == typeof(long))
                {
                    GameFrameWorkAction<GameObject, T, long> call = arg.call as GameFrameWorkAction<GameObject, T, long>;
                    call.Invoke(go, eventData, arg.longArg);
                }
                else if(arg.argType == typeof(float))
                {
                    GameFrameWorkAction<GameObject, T, float> call = arg.call as GameFrameWorkAction<GameObject, T, float>;
                    call.Invoke(go, eventData, arg.floatArg);
                }
                else if (arg.argType == typeof(double))
                {
                    GameFrameWorkAction<GameObject, T, double> call = arg.call as GameFrameWorkAction<GameObject, T, double>;
                    call.Invoke(go, eventData, arg.doubleArg);
                }
                else if (arg.argType == typeof(bool))
                {
                    GameFrameWorkAction<GameObject, T, bool> call = arg.call as GameFrameWorkAction<GameObject, T, bool>;
                    call.Invoke(go, eventData, arg.boolArg);
                }
            }
        }

        private bool HasCall(object call)
        {
            foreach(UIEventArg<T> arg in m_UIEventArgs)
            {
                if (arg.call == call)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveCall(object call)
        {
            for (int i = 0; i < m_UIEventArgs.Count; i++)
            {
                if(m_UIEventArgs[i].call == call)
                {
                    m_UIEventArgs[i].Release();
                    m_UIEventArgs.RemoveAt(i);
                    break;
                }
            }
        }

        private List<UIEventArg<T>> m_UIEventArgs = null;
    }
}