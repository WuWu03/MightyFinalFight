using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.FSM
{
    public class FSMMgr : BaseMgr<FSMMgr>
    {
        public int fsmCount
        {
            get
            {
                return m_ListFsms.Count;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_ListFsms = new List<FiniteStateMachine>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                m_ListFsms[i].Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();

            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                m_ListFsms[i].LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        protected override void OnFixedUpdate()
        {
            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                m_ListFsms[i].FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
            }
        }

        protected override void OnShutDown()
        {
            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                m_ListFsms[i].ShutDown();
            }

            m_ListFsms.Clear();
        }

        public FiniteStateMachine Create(object owner, string name, params BaseFsmState[] fsmStates)
        {
            if (HasFSM(owner))
            {
                Log.LogError("有限状态机[ ", name, "] 已经存在");
                return null;
            }

            FiniteStateMachine fsm = new FiniteStateMachine(owner, name, fsmStates);
            m_ListFsms.Add(fsm);
            return fsm;
        }

        public FiniteStateMachine GetFSM(object owner)
        {
            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                if (m_ListFsms[i].owner == owner)
                {
                    return m_ListFsms[i];
                }
            }

            return null;
        }

        public FiniteStateMachine[] GetAllFSM()
        {
            return m_ListFsms.ToArray() ;
        }

        public bool HasFSM(object owner)
        {
            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                if (m_ListFsms[i].owner == owner)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ReleaseFSM(object owner)
        {
            for (int i = m_ListFsms.Count - 1; i >= 0 ; i--)
            {
                if (m_ListFsms[i].owner == owner)
                {
                    m_ListFsms[i].ShutDown();
                    m_ListFsms.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool ReleaseFSM(FiniteStateMachine fsm)
        {
            if (fsm == null)
            {
                return false;
            }

            fsm.ShutDown();
            m_ListFsms.Remove(fsm);
            return true;
        }

        private List<FiniteStateMachine> m_ListFsms = null;
    }
}