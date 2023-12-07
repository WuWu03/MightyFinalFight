using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTree
    {
        public BehaviourTree(BehaviourTreeData data,object owner)
        {
            m_Root = Load(data, owner);
        }

        public void Start()
        {
            m_Root.Start();
            m_IsRunning = true;
            m_IsPause = false;
        }

        public void Update(float deltaTime)
        {
            if (!m_IsRunning || m_IsPause)
            {
                return;
            }

            m_Root.Update(deltaTime);
        }

        public void LateUpate(float deltaTime)
        {
            if (!m_IsRunning || m_IsPause)
            {
                return;
            }

            m_Root.LateUpdate(deltaTime);
        }

        public void Pasuse(bool value)
        {
            m_IsPause = value;
        }

        public void Stop()
        {
            if (!m_IsRunning)
            {
                return;
            }

            m_IsRunning = false;
            m_IsPause = false;
        }

        public void Destroy()
        {
            m_Root.Destroy();
        }

        private Node Load(BehaviourTreeData data, object owner)
        {
            Node root = BehaviourFactory.GetNodeByClassType(data.name, data.classType, data.args, owner, data.priority);

            if (data.preConditions != null && data.preConditions.Length > 0)
            {
                for (int i = 0; i < data.preConditions.Length; i++)
                {
                    if(root is Task)
                    {
                        (root as Task).AddPreCondition(BehaviourFactory.GetNodeByClassType(data.preConditions[i].name, data.preConditions[i].classType, data.preConditions[i].args, owner, 0));
                    }
                }
            }

            if (data.children != null && data.children.Length > 0)
            {
                for (int i = 0; i < data.children.Length; i++)
                {
                    if(root is ParentTask)
                    {
                        (root as ParentTask).AddChild(Load(data.children[i], owner) as Task);
                    }
                }
            }

            return root;
        }

        protected void Reset()
        {
            m_IsRunning = false;
            m_IsPause = false;
            m_Root.Reset();
        }

        private bool m_IsPause = false;
        private bool m_IsRunning = false;
        private Node m_Root = null;
    }
}
