using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

namespace FrameWork.BehaviourTree
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
            if (index > -1 && index < m_Childs.Count)
                return m_Childs[index];
            return null;
        }

        public override void Reset()
        {
            base.Reset();
            m_State = BehaviorTreeState.Running;
            for (int i = 0; i < m_Childs.Count; i++)
            {
                m_Childs[i].Reset();
            }
        }

        protected override void OnEnter() { }

        protected override void OnUpdate(float deltaTime) { }

        protected override void OnDestroy()
        {
            for (int i = 0; i < m_Childs.Count; i++)
            {
                m_Childs[i].Destroy();
            }

            for (int i = 0; i < m_PreConditions.Count; i++)
            {
                m_PreConditions[i].Destroy();
            }
        }

        protected int GetChildCount()
        {
            return m_Childs.Count;
        }

        private List<Node> m_Childs = null;
        protected List<Node> m_PreConditions = null;
    }
}
