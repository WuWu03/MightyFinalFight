using System;
using System.Collections.Generic;

namespace GameFrameWork.Event
{
    [Serializable]
    public abstract class GameFrameWorkBaseEvent<T, A> where T : GameFrameWorkBaseCall<A>, new()
    {
        protected HashSet<T> calls
        {
            get { return m_Calls; }
        }

        public GameFrameWorkBaseEvent()
        {
        }

        public void AddListener(A action)
        {
            if (HasListener(action))
            {
                Log.LogError("事件已经存在");
                return;
            }

            T call = ReferencePool.Acquire<T>();
            call.action = action;
            m_Calls.Add(call);
        }

        public void RemoveListener(A action)
        {
            T call = GetListener(action);
            call?.Release();
            m_Calls.Remove(call);
        }

        public void RemoveAllListeners()
        {
            foreach (T call in m_Calls)
            {
                call?.Release();
            }

            m_Calls.Clear();
        }

        public bool HasListener(A action)
        {
            if (action == null)
            {
                Log.LogError("事件不能为空");
                return false;
            }

            return GetListener(action) != null;
        }

        private T GetListener(A action)
        {
            foreach (T selfCall in m_Calls)
            {
                if (selfCall.action.Equals(action))
                {
                    return selfCall;
                }
            }

            return null;
        }

        private HashSet<T> m_Calls = new();
    }
}