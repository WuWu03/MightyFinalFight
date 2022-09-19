using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrameWork.Fsm
{
    public class FsmMgr : BaseMgr<FsmMgr>
    {
        public int fsmCount
        {
            get
            {
                return m_DicFsms.Count;
            }
        }

        private void Awake()
        {
            m_DicFsms = new Dictionary<object, BaseFsm>();
        }

        private void Update()
        {
            foreach (KeyValuePair<System.Object, BaseFsm> kvp in m_DicFsms)
            {
                if (kvp.Value.isRunning)
                {
                    kvp.Value.Update(Time.deltaTime, Time.unscaledDeltaTime);
                }
            }
        }

        public FsmMachine CreateFsm(System.Object owner, string name, params BaseFsmState[] fsmStates)
        {
            if (HasFsm(owner))
            {
                throw new Exception("The FSM mathine has exist.");
            }

            FsmMachine fsm = FsmMachine.Create(owner, name, fsmStates);
            m_DicFsms.Add(owner, fsm);
            return fsm;
        }

        public BaseFsm GetFsm(System.Object owner)
        {
            BaseFsm fsm = null;

            if (!m_DicFsms.TryGetValue(owner, out fsm))
            {
                return null;
            }

            return fsm;
        }

        public BaseFsm[] GetAllFsms()
        {
            BaseFsm[] fsmBases = new BaseFsm[m_DicFsms.Count];
            m_DicFsms.Values.CopyTo(fsmBases, 0);
            return fsmBases;
        }

        public bool HasFsm(System.Object owner)
        {
            return m_DicFsms.ContainsKey(owner);
        }

        public bool DestoryFsm(System.Object owner)
        {
            BaseFsm fsmBase = null;

            if (m_DicFsms.TryGetValue(owner, out fsmBase))
            {
                fsmBase.ShutDown();
                return m_DicFsms.Remove(owner);
            }

            return false;
        }

        protected override void OnShutDown()
        {
            m_DicFsms.Clear();
        }

        private Dictionary<System.Object, BaseFsm> m_DicFsms = null;
    }
}