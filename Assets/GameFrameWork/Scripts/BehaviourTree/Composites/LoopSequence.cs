//using System.Text.RegularExpressions;
//using UnityEngine;

//namespace GameFrameWork.BehaviourTree
//{
//    public class LoopSequence : Composite
//    {
//        public LoopSequence(int id, object owner, int priority, string args) : base(id, owner, priority, args)
//        {
//            m_CurrChildIndex = 0;
//            m_LastChildIndex = -1;
//            m_LoopTimes = 1;
//            m_CurrLoopTimes = 0;

//            if (!string.IsNullOrEmpty(args))
//            {
//                Match m = m_Regex.Match(args);

//                if (m.Success)
//                {
//                    m_LoopTimes = int.Parse(m.Groups[2].Value);
//                }

//                m_IsRandomLoop = m_LoopTimes == 0;
//            }
//        }

//        protected override void OnEnter()
//        {
//            m_CurrLoopTimes = 0;

//            if (m_IsRandomLoop)
//            {
//                m_LoopTimes = Random.Range(1, 9);
//            }
//        }

//        protected override void OnUpdate(float deltaTime)
//        {
//            Task child = GetChild(m_CurrChildIndex);
//            if (child != null)
//            {
//                if (CheckPreCondition() && child.CanExcute() && child.CheckPreCondition())
//                {
//                    if (m_CurrChildIndex != m_LastChildIndex)
//                    {
//                        m_LastChildIndex = m_CurrChildIndex;
//                        child.Enter();
//                    }

//                    child.Update(deltaTime);
//                    BehaviourTreeState state = child.Excute();
//                    if (state != BehaviourTreeState.Running)
//                    {
//                        m_CurrChildIndex++;
//                        if (state == BehaviourTreeState.Failure)
//                        {
//                            m_State = BehaviourTreeState.Failure;
//                            CheckLoopTimes();
//                        }
//                    }
//                }
//                else
//                {
//                    m_CurrChildIndex++;
//                }
//            }

//            if (m_CurrChildIndex >= GetChildCount())
//            {
//                CheckLoopTimes();
//            }
//        }

//        public override bool CanExcute()
//        {
//            return m_CurrChildIndex < GetChildCount() && m_State != BehaviourTreeState.Failure;
//        }

//        protected override void OnReset()
//        {
//            base.OnReset();
//            m_CurrChildIndex = 0;
//            m_LastChildIndex = -1;
//        }

//        private void CheckLoopTimes()
//        {
//            m_CurrLoopTimes++;

//            if (m_LoopTimes == -1 || m_CurrLoopTimes < m_LoopTimes)
//            {
//                Reset();
//            }
//        }

//        private bool m_IsRandomLoop = false;
//        private int m_CurrChildIndex = 0;
//        private int m_LastChildIndex = -1;
//        private int m_LoopTimes = 1;
//        private int m_CurrLoopTimes = 0;
//        private Regex m_Regex = new Regex(@"(LoopTimes:)(-?[0-9]+)");
//    }
//}