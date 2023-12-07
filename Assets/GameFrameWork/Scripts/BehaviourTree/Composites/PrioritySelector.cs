using GameFrameWork.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class PrioritySelector : Composite
    {
        public PrioritySelector(string name, string args, object owner, int priority) : base(name, args, owner, priority)
        {
            m_ListWeights = new List<int>();
            m_ListIndexes = new List<int>();
        }

        public override bool CanExcute()
        {
            return m_ListWeights.Count > 0 && m_State != BehaviourTreeState.Success;
        }

        public override BehaviourTreeState Excute()
        {
            if (m_State == BehaviourTreeState.Success)
            {
                return BehaviourTreeState.Success;
            }
            else if (m_ListWeights.Count <= 0)
            {
                return BehaviourTreeState.Failure;
            }

            return m_State;
        }

        protected override int GetCurrChildIndex()
        {
            return m_CurrChildIndex;
        }

        protected override void OnChildExcuteResult(int childIndex, BehaviourTreeState state)
        {
            m_State = state;

            m_ListWeights.RemoveAt(m_CurrRandomIndex);
            m_ListIndexes.RemoveAt(m_CurrRandomIndex);
            m_CurrRandomIndex = CommonUtil.RandomByWeight(m_ListWeights.ToArray());
            m_CurrChildIndex = m_ListIndexes[m_CurrRandomIndex];
        }

        protected override void OnStart()
        {
            base.OnStart();

            m_ListWeights.Clear();
            m_ListIndexes.Clear();

            for (int i = 0; i < GetChildCount(); i++)
            {
                m_ListWeights.Add(GetChild(i).priority);
                m_ListIndexes.Add(i);
            }
        }

        protected override void OnEnter()
        {
            m_State = BehaviourTreeState.Running;
            m_CurrRandomIndex = CommonUtil.RandomByWeight(m_ListWeights.ToArray());
            m_CurrChildIndex = m_ListIndexes[m_CurrRandomIndex];
        }

        protected override void OnReset()
        {
            base.OnReset();

            m_CurrChildIndex = 0;
            m_CurrRandomIndex = -1;
            m_State = BehaviourTreeState.None;

            m_ListWeights.Clear();
            m_ListIndexes.Clear();

            for (int i = 0; i < GetChildCount(); i++)
            {
                m_ListWeights.Add(GetChild(i).priority);
                m_ListIndexes.Add(i);
            }
        }

        private List<int> m_ListWeights = null;
        private List<int> m_ListIndexes = null;
        private int m_CurrChildIndex = 0;
        private int m_CurrRandomIndex = -1;
        private BehaviourTreeState m_State = BehaviourTreeState.None;
    }
}