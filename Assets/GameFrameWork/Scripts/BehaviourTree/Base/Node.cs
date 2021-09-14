using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
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

        public Node(string name, string args, object owner)
        {
            m_Name = name;
            m_Args = args;
            m_Owner = owner;
        }

        public void Enter()
        {
            OnEnter();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void Destroy()
        {
            OnDestroy();
            m_State = BehaviorTreeState.None;
            m_Name = string.Empty;
            m_Args = string.Empty;
            m_Owner = null;
        }

        public virtual void AddChild(Node node) { }
        public virtual Node GetChild(int index) { return null; }
        public virtual void AddPreCondition(Node node) { }
        public virtual bool CheckPreCondition() { return true; }
        public virtual void Reset() { m_State = BehaviorTreeState.Running; }
        public virtual bool CanExcute() { return true; }
        public virtual BehaviorTreeState Excute() { return m_State; }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(float deltaTime);
        protected abstract void OnDestroy();

        protected BehaviorTreeState m_State = BehaviorTreeState.None;
        protected string m_Name = string.Empty;
        protected string m_Args = string.Empty;
        protected object m_Owner = null;
    }
}
