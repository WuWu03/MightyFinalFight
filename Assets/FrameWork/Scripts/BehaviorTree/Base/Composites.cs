using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorTree
{
    public abstract class Composites : Node
    {
        public Composites(string name, string args, object owner) : base(name, args, owner)
        {
            m_Childs = new List<Node>();
            m_PreConditions = new List<Node>();
        }

        public override void AddPreCondition(Node node)
        {
            if (node == null) return;
            m_PreConditions.Add(node);
        }

        public override void AddChild(Node node)
        {
            if (node == null) return;
            m_Childs.Add(node);
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

        public override Node GetChild(int index)
        {
            return m_Childs[index];
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < m_Childs.Count; i++)
            {
                m_Childs[i].Reset();
            }
        }

        private List<Node> m_Childs = null;
        protected List<Node> m_PreConditions = null;
    }
}
