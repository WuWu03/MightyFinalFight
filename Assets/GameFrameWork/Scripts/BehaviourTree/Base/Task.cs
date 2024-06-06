using System.Collections.Generic;

namespace GameFrameWork.BehaviourTree
{
    public abstract class Task : BaseTask
    {
        public Task(string name, string args, object owner, int priority) : base(name, args, owner, priority)
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
                ParallelChlidren(deltaTime);
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

        private void ParallelChlidren(float deltaTime)
        {
            if (!CheckPreCondition() || !CanExcute())
            {
                BehaviourTreeState state = Excute();
                ExcuteResult(state);
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                BaseTask child = m_Children[i];
                ExcuteChild(child, i, deltaTime);
            }
        }

        private void SingleChild(float deltaTime)
        {
            if (!CheckPreCondition() || !CanExcute())
            {
                BehaviourTreeState state = Excute();
                ExcuteResult(state);
                return;
            }

            int childIndex = GetCurrChildIndex();
            BaseTask child = m_Children[childIndex];

            if (child != null)
            {
                ExcuteChild(child, childIndex, deltaTime);
            }
        }

        private void ExcuteChild(BaseTask child, int childIndex, float deltaTime)
        {
            child.Enter();

            if (child.CanExcute() && child.CheckPreCondition())
            {
                child.Update(deltaTime);
                BehaviourTreeState childState = child.Excute();

                if (childState != BehaviourTreeState.Running)
                {
                    ChildExcuteResult(childIndex, childState);
                }
            }
            else
            {
                ChildExcuteResult(childIndex, BehaviourTreeState.Failure);
            }
        }

        private void ExcuteResult(BehaviourTreeState state)
        {
            OnExcuteResult(state);
        }

        private void ChildExcuteResult(int childIndex, BehaviourTreeState state)
        {
            OnChildExcuteResult(childIndex, state);
        }

        protected virtual void OnExcuteResult(BehaviourTreeState state) { }
        protected virtual void OnChildExcuteResult(int childIndex, BehaviourTreeState state) { }

        private List<BaseTask> m_Children = null;
    }
}