namespace GameFrameWork.BehaviourTree
{
    public class Sequence : Composite
    {
        public Sequence(int id, object owner, int priority, string args) : base(id, owner, priority, args) 
        {
            m_CurrChildIndex = 0;
        }

        public override BehaviourTreeState Excute()
        {
            if(m_State == BehaviourTreeState.Failure)
            {
                return BehaviourTreeState.Failure;
            }
            else if(m_CurrChildIndex >= GetChildCount())
            {
                return BehaviourTreeState.Success;
            }

            return BehaviourTreeState.Running;
        }

        public override bool CanExecute()
        {
            return m_CurrChildIndex < GetChildCount() && m_State != BehaviourTreeState.Failure;
        }

        protected override void OnChildExecuteResult(int childIndex, BehaviourTreeState state)
        {
            base.OnChildExecuteResult(childIndex, state);

            m_CurrChildIndex++;
            m_State = state;
        }

        protected override int GetCurrChildIndex()
        {
            return m_CurrChildIndex;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            m_State = BehaviourTreeState.Running;
        }

        protected override void OnReset()
        {
            base.OnReset();
            m_CurrChildIndex = 0;
            m_State = BehaviourTreeState.None;
        }

        private BehaviourTreeState m_State = BehaviourTreeState.None;
        private int m_CurrChildIndex;
    }
}