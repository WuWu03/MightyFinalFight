using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Fsm
{
    public class FsmMgr : BaseMgr<FsmMgr>
    {
        public int fsmCount
        {
            get
            {
                return m_ListFsms.Count;
            }
        }

        public FsmMachine Create(System.Object owner, string name, params BaseFsmState[] fsmStates)
        {
            if (HasFsm(owner))
            {
                throw new Exception("The fsm mathine has exist.");
            }

            FsmMachine fsm = new FsmMachine(owner, name, fsmStates);
            m_ListFsms.Add(fsm);
            return fsm;
        }

        public FsmMachine GetFsm(System.Object owner)
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

        public FsmMachine[] GetAllFsms()
        {
            return m_ListFsms.ToArray() ;
        }

        public bool HasFsm(System.Object owner)
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

        public bool DestoryFsm(System.Object owner)
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

        public bool DestoryFsm(FsmMachine fsm)
        {
            if(fsm == null)
            {
                return false;
            }

            fsm.ShutDown();
            m_ListFsms.Remove(fsm);
            return true;
        }

        protected override void OnAwake()
        {
            m_ListFsms = new List<FsmMachine>();
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < m_ListFsms.Count; i++)
            {
                m_ListFsms[i].Update(Time.deltaTime, Time.unscaledDeltaTime);
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

        private List<FsmMachine> m_ListFsms = null;
    }
}