using System.Collections.Generic;

namespace GameFrameWork.Fsm
{
    public class FsmMgr : BaseMgr<FsmMgr>
    {
        public int fsmCount
        {
            get
            {
                return m_Fsms.Count;
            }
        }

        protected override void OnAwake()
        {
            m_Fsms = new();
        }

        protected override void OnShutDown()
        {
            foreach (KeyValuePair<object, Fsm> fsm in m_Fsms)
            {
                fsm.Value.Release();
            }

            m_Fsms.Clear();
        }

        protected override void OnDestory()
        {
            m_Fsms = null;
        }

        public Fsm CreateFsm(object owner, string name)
        {
            if (owner == null)
            {
                Log.LogError("有限状态机持有者为空");
                return null;
            }

            if (HasFsm(owner))
            {
                Log.LogError("已经存在相同的有限状态机，请勿重复创建");
                return null;
            }

            Fsm fsm = Fsm.Create(owner, name);
            m_Fsms.Add(owner, fsm);
            return fsm;
        }

        public Fsm GetFsm(object owner, string name)
        {
            if (owner == null)
            {
                Log.LogError("有限状态机持有者为空");
                return null;
            }

            if (m_Fsms.TryGetValue(new TypeNamePair(owner.GetType(), name), out Fsm fsm))
            {
                return fsm;
            }

            return null;
        }

        public bool HasFsm(object owner)
        {
            if (owner == null)
            {
                Log.LogError("有限状态机持有者为空");
                return false;
            }

            return m_Fsms.ContainsKey(owner);
        }

        public void ReleaseFsm(object owner)
        {
            if (owner == null)
            {
                Log.LogError("有限状态机持有者为空");
                return;
            }

            if (m_Fsms.TryGetValue(owner, out Fsm fsm))
            {
                fsm.Release();
                m_Fsms.Remove(owner);
            }
        }

        public void ReleaseFsm(Fsm fsm)
        {
            if (fsm == null)
            {
                return;
            }

            ReleaseFsm(fsm.owner);
        }

        private Dictionary<object, Fsm> m_Fsms = null;
    }
}