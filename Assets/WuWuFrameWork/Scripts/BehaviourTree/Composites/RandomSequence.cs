using UnityEngine;


namespace WuWuFramework.BehaviourTree
{
    public class RandomSequence : Composite
    {
        public RandomSequence(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {

        }

        public override BehaviourTreeState Excute()
        {
            if (m_State == BehaviourTreeState.Failure)
            {
                return BehaviourTreeState.Failure;
            }
            else if (m_CurrChildIndex >= GetChildCount())
            {
                return BehaviourTreeState.Success;
            }

            return BehaviourTreeState.Running;
        }

        public override bool CanExecute()
        {
            return m_CurrChildIndex < GetChildCount() && m_State != BehaviourTreeState.Failure;
        }

        protected override int GetCurrChildIndex()
        {
            return m_ChildrenIndexes[m_CurrChildIndex];
        }

        protected override void OnChildExecuteResult(int childIndex, BehaviourTreeState state)
        {
            base.OnChildExecuteResult(childIndex, state);
            m_State = state;
            m_CurrChildIndex++;
        }

        protected override void OnStart()
        {
            base.OnStart();

            m_ChildrenIndexes = new int[GetChildCount()];
            m_CurrChildIndex = 0;

            for (int i = 0; i < GetChildCount(); i++)
            {
                m_ChildrenIndexes[i] = i;
            }

            ShuffChildren();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            m_CurrChildIndex = 0;
            m_State = BehaviourTreeState.Running;
        }

        protected override void OnReset()
        {
            base.OnReset();

            m_CurrChildIndex = 0;
            m_State = BehaviourTreeState.None;

            ShuffChildren();
        }

        private void ShuffChildren()
        {
            for (int i = m_ChildrenIndexes.Length; i > 0; --i)
            {
                int j = Random.Range(0, i);
                int childIndex = m_ChildrenIndexes[j];
                m_ChildrenIndexes[j] = m_ChildrenIndexes[i - 1];
                m_ChildrenIndexes[i - 1] = childIndex;
            }
        }

        private int m_CurrChildIndex = 0;
        private BehaviourTreeState m_State = BehaviourTreeState.None;
        private int[] m_ChildrenIndexes = null;
    }
}