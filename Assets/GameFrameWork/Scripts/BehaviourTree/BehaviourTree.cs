using System;
using GameFrameWork.Utils;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTree
    {
        private bool m_IsPause;
        private bool m_IsRunning;
        private readonly Node m_Root;

        public BehaviourTree(BehaviourTreeData data, object owner)
        {
            m_Root = Load(data, owner);
            m_IsRunning = false;
            m_IsPause = false;
        }

        public Node tree
        {
            get
            {
                return m_Root;
            }
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
            Node root = GetNodeByClassType(data.id, owner, data.priority, data.args, data.classType);

            if (data.preConditions is { Length: > 0 })
            {
                foreach (var preCondition in data.preConditions)
                {
                    if (root is BaseTask baseTask)
                    {
                        baseTask.AddPreCondition(GetPreConditionNodeByClassType(preCondition.id, owner, preCondition.priority, preCondition.isAndCondition, preCondition.args, preCondition.classType));
                    }
                }
            }

            if (data.children is { Length: > 0 })
            {
                foreach (var child in data.children)
                {
                    if (root is Task task)
                    {
                        task.AddChild(Load(child, owner) as BaseTask);
                    }
                }
            }

            return root;
        }

        private Node GetNodeByClassType(int id, object owner, int priority, string args, string className)
        {
            Type t = GetNodeType(id, className);
            return t == null ? null : (Node)System.Activator.CreateInstance(t, id, owner, priority, args);
        }

        private PreCondition GetPreConditionNodeByClassType(int id, object owner, int priority, bool isAndCondition, string args, string className)
        {
            Type t = GetNodeType(id, className);
            return t == null ? null : (PreCondition)System.Activator.CreateInstance(t, id, owner, priority, isAndCondition, args);
        }

        private Type GetNodeType(int id, string className)
        {
            Type t = Type.GetType("GameFrameWork.BehaviourTree." + className) ?? Type.GetType(className);

            if (t == null)
            {
                throw new GameFrameWorkException(StringUtil.Append("行为树数据实例不存在 : [", id.ToString(), "]"));
            }

            return t;
        }
    }
}