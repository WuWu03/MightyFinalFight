using System.Text.RegularExpressions;


namespace GameFrameWork.BehaviourTree
{
    public class Selector : Composites
    {
        public Selector(string name, string args, object owner) : base(name, args, owner)
        {
            m_CurrChildIndex = 0;
            m_LastChildIndex = -1;
        }

        protected override void OnUpdate(float deltaTime)
        {
            Node child = GetChild(m_CurrChildIndex);
            if (child != null)
            {
                if (child.CanExcute() && child.CheckPreCondition()&&this.CheckPreCondition())
                {
                    if (m_CurrChildIndex != m_LastChildIndex)
                    {
                        m_LastChildIndex = m_CurrChildIndex;
                        child.Enter();
                    }

                    child.Update(deltaTime);
                    BehaviorTreeState state = child.Excute();
                    if (state != BehaviorTreeState.Running)
                    {
                        m_CurrChildIndex++;
                        if (state == BehaviorTreeState.Success)
                        {
                            m_State = BehaviorTreeState.Success;
                            return;
                        }
                    }
                }
                else
                {
                    m_CurrChildIndex++;
                }
            }

            if (m_CurrChildIndex >= GetChildCount())
            {
                m_State = BehaviorTreeState.Failure;
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

        private int m_CurrChildIndex;
        private int m_LastChildIndex;
    }
}