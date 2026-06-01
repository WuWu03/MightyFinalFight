using System.Collections.Generic;

namespace WuWuFramework.Fsm
{
    public class FsmMgr : WuWuFrameworkModule, IFsmMgr
    {
        private readonly Dictionary<object, Fsm> m_Fsms;
        
        public FsmMgr()
        {
            m_Fsms = new();
        }
        
        public int fsmCount
        {
            get { return m_Fsms.Count; }
        }
        
        public override void Shutdown()
        {
            foreach (KeyValuePair<object, Fsm> fsm in m_Fsms)
            {
                fsm.Value.Release();
            }

            m_Fsms.Clear();
        }

        public Fsm CreateFsm(object owner, string name)
        {
            if (owner == null)
            {
                throw new WuWuFrameworkException("有限状态机持有者为空");
            }

            if (HasFsm(owner))
            {
                throw new WuWuFrameworkException("已经存在相同的有限状态机，请勿重复创建");
            }

            Fsm fsm = Fsm.Create(owner, name);
            m_Fsms.Add(owner, fsm);
            return fsm;
        }

        public Fsm GetFsm(object owner, string name)
        {
            if (owner == null)
            {
                throw new WuWuFrameworkException("有限状态机持有者为空");
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
                throw new WuWuFrameworkException("有限状态机持有者为空");
            }

            return m_Fsms.ContainsKey(owner);
        }

        public void ReleaseFsm(object owner)
        {
            if (owner == null)
            {
                throw new WuWuFrameworkException("有限状态机持有者为空");
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
    }
}