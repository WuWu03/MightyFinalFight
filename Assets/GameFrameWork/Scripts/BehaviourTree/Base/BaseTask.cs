using System.Collections.Generic;

namespace GameFrameWork.BehaviourTree
{
    public abstract class BaseTask : Node
    {
        protected BaseTask(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
        {
            m_PreConditions = new List<PreCondition>();
        }

        public virtual bool CanExcute()
        {
            return true;
        }

        public virtual BehaviourTreeState Excute()
        {
            return BehaviourTreeState.Running;
        }

        public void AddPreCondition(PreCondition preCondition)
        {
            if (preCondition == null || m_PreConditions == null)
            {
                return;
            }

            m_PreConditions.Add(preCondition);
        }

        public bool CheckPreCondition()
        {
            bool result = true;
            bool andCondition = false;

            for (int i = 0; i < m_PreConditions.Count; i++)
            {
                bool condition = m_PreConditions[i].CheckPreCondition();

                if (m_PreConditions[i].isAndCondition)
                {
                    andCondition = andCondition || condition;
                }
                else
                {
                    result = result && condition;
                }
            }

            return result || andCondition;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < m_PreConditions.Count; i++)
            {
                m_PreConditions[i].Destroy();
            }
        }

        private List<PreCondition> m_PreConditions = null;
    }
}
