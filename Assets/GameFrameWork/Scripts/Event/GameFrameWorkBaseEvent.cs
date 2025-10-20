using System.Collections.Generic;

namespace GameFrameWork.Event
{
    public abstract class GameFrameWorkBaseEvent<T, A> where T : GameFrameWorkBaseCall<A>, new()
    {
        private readonly List<T> m_Calls;
        private readonly List<T> m_PresisttentCalls;
        private bool m_IsCallDirty;
        protected List<T> calls
        {
            get { return m_PresisttentCalls; }
        }

        public GameFrameWorkBaseEvent()
        {
            m_Calls = new();
            m_PresisttentCalls = new();
        }

        public void AddListener(A action)
        {
            if (HasListener(action))
            {
                throw new GameFrameWorkException("事件已经存在");
            }

            T call = ReferencePool.Acquire<T>();
            call.action = action;
            m_Calls.Add(call);
            m_IsCallDirty = true;
        }

        public void RemoveListener(A action)
        {
            T call = GetListener(action);
            call?.Release();
            m_Calls.Remove(call);
            m_IsCallDirty = true;
        }

        public void RemoveAllListeners()
        {
            foreach (T call in m_Calls)
            {
                call?.Release();
            }

            m_Calls.Clear();
            m_IsCallDirty = true;
        }

        public bool HasListener(A action)
        {
            if (action == null)
            {
                throw new GameFrameWorkException("事件不能为空");
            }

            return GetListener(action) != null;
        }

        protected void RebuildCallListIfNeeded()
        {
            if (!m_IsCallDirty)
            {
                return;
            }

            m_IsCallDirty = false;
            m_PresisttentCalls.Clear();
            m_PresisttentCalls.AddRange(m_Calls);
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
    }
}