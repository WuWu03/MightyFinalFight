using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorTree
{
    public enum BehaviorTreeState
    {
        None = 0,
        Success = 1,
        Running = 2,
        Failure = 3,
    }

    public abstract class Node
    {
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public Node(string name,string args)
        {
            m_Childs = new List<Node>();
            m_PreConditions = new List<Node>();
            m_Name = name;
            m_Args = args;
        }

        public void Enter()
        {
            OnEnter();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void AddChild(Node node)
        {
            if (node == null) return;
            m_Childs.Add(node);
        }

        public void AddPreCondition(Node node)
        {
            if (node == null) return;
            m_PreConditions.Add(node);
        }

        public Node GetChild(int index)
        {
            return m_Childs[index];
        }

        public virtual bool CheckPreCondition()
        {
            for(int i = 0; i < m_PreConditions.Count; i++)
            {
                if (!m_PreConditions[i].CheckPreCondition())
                {
                    return false;
                }
            }

            return true;
        }
    
        public virtual void Reset()
        {
            m_State = BehaviorTreeState.Running;
            for(int i = 0; i < m_Childs.Count; i++)
            {
                m_Childs[i].Reset();
            }
        }

        public virtual BehaviorTreeState Do()
        {
            return BehaviorTreeState.None;
        }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(float deltaTime);

        protected BehaviorTreeState m_State = BehaviorTreeState.None;
        protected string m_Name = string.Empty;
        protected string m_Args = string.Empty;
        private List<Node> m_PreConditions = null;
        private List<Node> m_Childs = null;
    }
}
