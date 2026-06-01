using System.Collections.Generic;

namespace WuWuFramework.BehaviourTree
{
    public abstract class Task : BaseTask
    {
        private readonly List<BaseTask> m_Children;
        
        public Task(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {
            m_Children = new List<BaseTask>();
        }
        
        public void AddChild(BaseTask node)
        {
            if (node == null || m_Children == null)
            {
                return;
            }

            m_Children.Add(node);
        }

        public BaseTask GetChild(int index)
        {
            if (index > -1 && index < m_Children.Count)
            {
                return m_Children[index];
            }

            return null;
        }

        public List<BaseTask> GetChildren()
        {
            if (m_Children == null)
            {
                return null;
            }

            return m_Children;
        }

        protected override void OnStart()
        {
            if (m_Children == null)
            {
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Start();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (CanRunParallelChildren())
            {
                ParallelChildren(deltaTime);
            }
            else
            {
                SingleChild(deltaTime);
            }
        }

        protected override void OnReset()
        {
            base.OnReset();

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Reset();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].Destroy();
            }
        }

        protected int GetChildCount()
        {
            return m_Children.Count;
        }

        protected virtual int GetCurrChildIndex()
        {
            return 0;
        }

        protected virtual bool CanRunParallelChildren()
        {
            return false;
        }

        private void ParallelChildren(float deltaTime)
        {
            if (!CheckPreCondition() || !CanExecute())
            {
                BehaviourTreeState state = Excute();
                ExecuteResult(state);
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                BaseTask child = m_Children[i];
                ExecuteChild(child, i, deltaTime);
            }
        }

        private void SingleChild(float deltaTime)
        {
            if (!CheckPreCondition() || !CanExecute())
            {
                BehaviourTreeState state = Excute();
                ExecuteResult(state);
                return;
            }

            int childIndex = GetCurrChildIndex();
            BaseTask child = m_Children[childIndex];

            if (child != null)
            {
                ExecuteChild(child, childIndex, deltaTime);
            }
        }

        private void ExecuteChild(BaseTask child, int childIndex, float deltaTime)
        {
            child.Enter();

            if (child.CanExecute() && child.CheckPreCondition())
            {
                child.Update(deltaTime);
                BehaviourTreeState childState = child.Excute();

                if (childState != BehaviourTreeState.Running)
                {
                    ChildExecuteResult(childIndex, childState);
                }
            }
            else
            {
                ChildExecuteResult(childIndex, BehaviourTreeState.Failure);
            }
        }

        private void ExecuteResult(BehaviourTreeState state)
        {
            OnExecuteResult(state);
        }

        private void ChildExecuteResult(int childIndex, BehaviourTreeState state)
        {
            OnChildExecuteResult(childIndex, state);
        }

        protected virtual void OnExecuteResult(BehaviourTreeState state) { }
        protected virtual void OnChildExecuteResult(int childIndex, BehaviourTreeState state) { }
    }
}