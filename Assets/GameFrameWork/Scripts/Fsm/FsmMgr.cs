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
                return m_FSMList.Count;
            }
        }

        public int unUsedFSMCount
        {
            get
            {
                return m_FSMQueue.Count;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_FSMQueue = new Queue<FiniteStateMachine>();
            m_FSMList = new List<FiniteStateMachine>();
        }

        protected override void OnShutDown()
        {
            for (int i = 0; i < m_FSMList.Count; i++)
            {
                m_FSMList[i].Release();
            }

            m_FSMList.Clear();
            m_FSMQueue.Clear();
            m_FSMList = null;
            m_FSMQueue = null;
        }

        public FiniteStateMachine CreateFSM(object owner, string name)
        {
            FiniteStateMachine fsm = null;

            if (m_FSMQueue.Count > 0)
            {
                fsm = m_FSMQueue.Dequeue();
                fsm.ResetInfo(owner, name);
            }

            if (fsm == null)
            {
                fsm = new FiniteStateMachine(owner, name);
            }

            m_FSMList.Add(fsm);
            return fsm;
        }

        public FiniteStateMachine GetFSM(object owner)
        {
            for (int i = 0; i < m_FSMList.Count; i++)
            {
                if (m_FSMList[i].owner == owner)
                {
                    return m_FSMList[i];
                }
            }

            return null;
        }

        public bool HasFSM(object owner)
        {
            for (int i = 0; i < m_FSMList.Count; i++)
            {
                if (m_FSMList[i].owner == owner)
                {
                    return true;
                }
            }

            return false;
        }

        public void ReleaseFSM(FiniteStateMachine fsm)
        {
            if (fsm == null)
            {
                return;
            }

            fsm.Release();
            m_FSMQueue.Enqueue(fsm);
            m_FSMList.Remove(fsm);
        }

        private List<FiniteStateMachine> m_FSMList = null;
        private Queue<FiniteStateMachine> m_FSMQueue = null;
    }
}