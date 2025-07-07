using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public abstract class BaseTask : Node
    {
        protected BaseTask(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
        {
            m_PreConditions = new List<Node>();
        }

        public virtual bool CanExcute()
        {
            return true;
        }

        public virtual BehaviourTreeState Excute()
        {
            return BehaviourTreeState.Running;
        }

        public void AddPreCondition(Node node)
        {
            if (node == null || m_PreConditions == null)
            {
                return;
            }

            m_PreConditions.Add(node);
        }

        public override bool CheckPreCondition()
        {
            for (int i = 0; i < m_PreConditions.Count; i++)
            {
                if (!m_PreConditions[i].CheckPreCondition())
                {
                    return false;
                }
            }

            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < m_PreConditions.Count; i++)
            {
                m_PreConditions[i].Destroy();
            }
        }

        private List<Node> m_PreConditions = null;
    }
}
