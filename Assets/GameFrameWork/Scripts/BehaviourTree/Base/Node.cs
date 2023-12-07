using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public enum BehaviourTreeState
    {
        None = 0,
        Success = 1,
        Running = 2,
        Failure = 3,
    }

    public abstract class Node
    {
        public string name
        {
            get
            {
                return m_Name;
            }
        }

        public int priority
        {
            get
            {
                return m_Priority;
            }
        }

        public Node(string name, string args, object owner, int priority)
        {
            m_Name = name;
            m_Args = args;
            m_Owner = owner;
            m_Priority = priority;
        }

        public void Start()
        {
            OnStart();
        }

        public void Enter()
        {
            if (m_HasEnter)
            {
                return;
            }

            m_HasEnter = true;
            OnEnter();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            OnLateUpdate(deltaTime);
        }

        public void Destroy()
        {
            OnDestroy();

            m_Name = string.Empty;
            m_Args = string.Empty;
            m_Owner = null;
        }

        public void Reset()
        {
            m_HasEnter = false;
            OnReset();
        }

        public virtual bool CheckPreCondition() 
        {
            return true; 
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnter() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnLateUpdate(float deltaTime) { }
        protected virtual void OnDestroy() { }
        protected virtual void OnReset() { }

        protected string m_Name = string.Empty;
        protected string m_Args = string.Empty;
        protected object m_Owner = null;
        protected int m_Priority = 0;

        private bool m_HasEnter = false;
    }
}
