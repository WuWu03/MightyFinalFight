using System.Text.RegularExpressions;


namespace GameFrameWork.BehaviourTree
{
    public class LoopSelector : Composites
    {
        public LoopSelector(string name, string args, object owner) : base(name, args, owner) 
        {
            m_CurrChildIndex = 0;
            m_LastChildIndex = -1;
            m_CurrLoopTimes = 0;
            m_LoopTimes = 1;

            if (!string.IsNullOrEmpty(args))
            {
                Match m = m_Regex.Match(args);
                if (m.Success) m_LoopTimes = int.Parse(m.Groups[2].Value);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            Node child = GetChild(m_CurrChildIndex);
            if(child != null)
            {
                if(child.CanExcute() && child.CheckPreCondition())
                {
                    if(m_CurrChildIndex != m_LastChildIndex)
                    {
                        m_LastChildIndex = m_CurrChildIndex;
                        child.Enter();
                    }

                    child.Update(deltaTime);
                    BehaviorTreeState state = child.Excute();
                    if(state != BehaviorTreeState.Running)
                    {
                        m_CurrChildIndex++;
                        if (state == BehaviorTreeState.Success)
                        {
                            m_State = BehaviorTreeState.Success;
                            CheckLoopTimes();
                        }
                    }
                }
                else
                {
                    m_CurrChildIndex++;
                }
            }

            if(m_CurrChildIndex >= GetChildCount())
            {
                CheckLoopTimes();
            }
        }

        public override bool CanExcute()
        {
            return m_CurrChildIndex < GetChildCount() && m_State != BehaviorTreeState.Success;
        }

        public override void Reset()
        {
            base.Reset();
            m_CurrChildIndex = 0;
            m_LastChildIndex = -1;
        }

        private void CheckLoopTimes()
        {
            m_CurrLoopTimes++;
            if(m_LoopTimes == -1 || m_CurrLoopTimes < m_LoopTimes)
            {
                Reset();
            }
        }

        private int m_CurrChildIndex;
        private int m_LastChildIndex;
        private int m_LoopTimes;
        private int m_CurrLoopTimes;
        private Regex m_Regex = new Regex(@"(LoopTimes:)([0-9]+)");
    }
}