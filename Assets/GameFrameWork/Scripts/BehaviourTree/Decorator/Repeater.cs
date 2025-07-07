namespace GameFrameWork.BehaviourTree
{
    public class Repeater : Decorator
    {
        public Repeater(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
        {
            m_RepeatCount = 0;
        }

        public override bool CanExcute()
        {
            return m_RepeatCount == 0 || m_CurrRepeatCount <= m_RepeatCount;
        }

        protected override void OnExcuteResult(BehaviourTreeState state)
        {
            base.OnExcuteResult(state);
            Reset();
        }

        protected override void OnChildExcuteResult(int childIndex, BehaviourTreeState state)
        {
            base.OnChildExcuteResult(childIndex, state);

            m_CurrExuteCount++;

            if (m_CurrExuteCount >= GetChildCount())
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
            m_CurrExuteCount = 0;
            m_CurrRepeatCount = 0;
        }

        private int m_CurrExuteCount = 0;
        private int m_CurrRepeatCount = 0;
        private int m_RepeatCount = 0;
    }
}

