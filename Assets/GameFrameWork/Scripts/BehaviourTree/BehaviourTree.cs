using System;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTree
    {
        public Node tree
        {
            get
            {
                return m_Root;
            }
        }

        public BehaviourTree(BehaviourTreeData data, object owner)
        {
            m_Root = Load(data, owner);
            m_IsRunning = false;
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

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!m_IsRunning || m_IsPause)
            {
                return;
            }

            m_Root.FixedUpdate(fixedDeltaTime);
        }

        public void Start()
        {
            if (m_IsRunning)
            {
                return;
            }

            m_Root.Start();
            m_IsRunning = true;
            m_IsPause = false;
        }

        public void Pause()
        {
            m_IsPause = true;
        }

        public void Resume()
        {
            m_IsPause = false;
        }

        public void Stop()
        {
            if (!m_IsRunning)
            {
                return;
            }

            m_IsRunning = false;
            m_IsPause = false;
            m_Root.Reset();
        }

        public void Destroy()
        {
            m_IsRunning = false;
            m_IsPause = false;
            m_Root.Destroy();
        }

        private Node Load(BehaviourTreeData data, object owner)
        {
            Node root = GetNodeByClassType(data.name, data.id, owner, data.priority, data.args, data.classType);

            if (data.preConditions != null && data.preConditions.Length > 0)
            {
                for (int i = 0; i < data.preConditions.Length; i++)
                {
                    if (root is BaseTask)
                    {
                        (root as BaseTask).AddPreCondition(GetNodeByClassType(data.preConditions[i].name, data.preConditions[i].id, owner, 0, data.preConditions[i].args, data.preConditions[i].classType));
                    }
                }
            }

            if (data.children != null && data.children.Length > 0)
            {
                for (int i = 0; i < data.children.Length; i++)
                {
                    if (root is Task)
                    {
                        (root as Task).AddChild(Load(data.children[i], owner) as BaseTask);
                    }
                }
            }

            return root;
        }

        private Node GetNodeByClassType(string name, int id, object owner, int priority, string args, string className)
        {
            Type t = Type.GetType("GameFrameWork.BehaviourTree." + className);

            if (t == null)
            {
                t = Type.GetType(className);
            }

            if (t == null)
            {
                Log.LogError("行为树数据实例不存在 : " + name);
                return null;
            }

            return (Node)System.Activator.CreateInstance(t, name, id, owner, priority, args);
        }

        private bool m_IsPause = false;
        private bool m_IsRunning = false;
        private Node m_Root = null;
    }
}