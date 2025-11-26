using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class Repeater : Decorator
    {
        private int m_CurrExecuteCount;
        private int m_CurrRepeatTimes;
        private readonly int m_OriginalRepeatTimes;
        private int m_RepeatTimes;
        private bool m_IsRandomRepeat;

        public Repeater(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {
            Regex mRegex = new("(RepeatTime:)(-?[0-9]+)");

            if (!string.IsNullOrEmpty(args))
            {
                Match m = mRegex.Match(args);
                if (m.Success)
                {
                    int repeatTimes = int.Parse(m.Groups[2].Value);
                    m_IsRandomRepeat = repeatTimes < 0;
                    m_OriginalRepeatTimes = Mathf.Abs(repeatTimes);
                }
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            m_RepeatTimes = m_OriginalRepeatTimes;
            
            if (m_IsRandomRepeat)
            {
                m_RepeatTimes = Random.Range(0, m_OriginalRepeatTimes + 1);
            }
        }

        public override bool CanExecute()
        {
            return m_RepeatTimes == 0 || m_CurrRepeatTimes <= m_RepeatTimes;
        }

        protected override void OnExecuteResult(BehaviourTreeState state)
        {
            if (state != BehaviourTreeState.Running)
            {
                Reset();
            }
        }

        protected override void OnChildExecuteResult(int childIndex, BehaviourTreeState state)
        {
            if (state != BehaviourTreeState.Running)
            {
                m_CurrExecuteCount++;

                if (m_CurrExecuteCount >= GetChildCount())
                {
                    if (m_RepeatTimes == 0)
                    {
                        Reset();
                        return;
                    }

                    m_CurrRepeatTimes++;
                }
            }
        }

        protected override void OnReset()
        {
            base.OnReset();
            m_CurrExecuteCount = 0;
            m_CurrRepeatTimes = 0;
        }
    }
}