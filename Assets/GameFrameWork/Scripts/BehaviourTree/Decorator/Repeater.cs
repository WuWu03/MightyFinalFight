namespace GameFrameWork.BehaviourTree
{
    public class Repeater : Decorator
    {
        private int m_CurrExecuteCount;
        private int m_CurrRepeatCount;
        private readonly int m_RepeatCount;
        
        public Repeater(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {
            m_RepeatCount = 0;
        }

        public override bool CanExecute()
        {
            return m_RepeatCount == 0 || m_CurrRepeatCount <= m_RepeatCount;
        }

        protected override void OnExecuteResult(BehaviourTreeState state)
        {
            base.OnExecuteResult(state);
            Reset();
        }

        protected override void OnChildExecuteResult(int childIndex, BehaviourTreeState state)
        {
            base.OnChildExecuteResult(childIndex, state);

            m_CurrExecuteCount++;

            if (m_CurrExecuteCount >= GetChildCount())
            {
                if (m_RepeatCount == 0)
                {
                    Reset();
                    return;
                }

                m_CurrRepeatCount++;
            }
        }

        protected override void OnReset()
        {
            base.OnReset();
            m_CurrExecuteCount = 0;
            m_CurrRepeatCount = 0;
        }
    }
}

