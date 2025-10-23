namespace GameFrameWork.BehaviourTree
{
    public class Selector : Composite
    {
        public Selector(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {
            m_CurrChildIndex = 0;
        }

        public override BehaviourTreeState Excute()
        {
            if (m_State == BehaviourTreeState.Success)
            {
                return BehaviourTreeState.Success;
            }
            
            if (m_CurrChildIndex >= GetChildCount())
            {
                return BehaviourTreeState.Failure;
            }

            return BehaviourTreeState.Running;
        }

        public override bool CanExecute()
        {
            return m_CurrChildIndex < GetChildCount() && m_State != BehaviourTreeState.Success;
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
        }

        private int m_CurrChildIndex;
        private BehaviourTreeState m_State = BehaviourTreeState.None;
    }
}