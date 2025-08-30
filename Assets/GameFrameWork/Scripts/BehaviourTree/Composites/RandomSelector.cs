using UnityEngine;


namespace GameFrameWork.BehaviourTree
{
    public class RandomSelector : Composite
    {
        public RandomSelector(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {

        }

        public override BehaviourTreeState Excute()
        {
            if (m_State == BehaviourTreeState.Success)
            {
                return BehaviourTreeState.Success;
            }
            else if (m_CurrChildIndex >= GetChildCount())
            {
                return BehaviourTreeState.Failure;
            }

            return BehaviourTreeState.Running;
        }

        public override bool CanExcute()
        {
            return m_CurrChildIndex < GetChildCount() && m_State != BehaviourTreeState.Success;
        }

        protected override int GetCurrChildIndex()
        {
            return m_ChildrenIndexes[m_CurrChildIndex];
        }

        protected override void OnChildExcuteResult(int childIndex, BehaviourTreeState state)
        {
            base.OnChildExcuteResult(childIndex, state);
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
            // 1 2 3 4 5
            //5 2 3 4 1
            //5 4 3 2 1
            //5 3 4 2 1

            for (int i = m_ChildrenIndexes.Length; i > 0; --i)
            {
                int j = Random.Range(0, i - 1);
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