using GameFrameWork.Utility;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class RandomSelector : Composites
    {
        public RandomSelector(string name, string args, object owner) : base(name, args, owner)
        {
            m_CurrChildIndex = 0;
            m_LastChildIndex = -1;
            m_ListWeight = new List<int>();

            if (!string.IsNullOrEmpty(args))
            {
                Match m = m_Regex.Match(args);
                if (m.Success)
                {
                    string[] str = m.Groups[3].Value.Split(',');
                    m_Weights = new int[str.Length];

                    for (int i = 0; i < str.Length; i++)
                    {
                        m_Weights[i] = str[i].ToInt();
                    }

                    m_ListWeight.AddRange(m_Weights);
                }
            }
            else
            {
                for (int i = 0; i < GetChildCount(); i++)
                {
                    m_ListWeight.Add(1);
                }
            }
        }

        protected override void OnEnter()
        {
            m_ListWeight.Clear();
            m_ListWeight.AddRange(m_Weights);
            m_CurrChildIndex = Util.RandomByWeight(m_ListWeight.ToArray());
            m_ListWeight.Remove(m_CurrChildIndex);
            m_LastChildIndex = -1;
        }

        protected override void OnUpdate(float deltaTime)
        {
            Node child = GetChild(m_CurrChildIndex);

            if (child != null)
            {
                if (child.CanExcute() && child.CheckPreCondition() && this.CheckPreCondition())
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
                        m_CurrChildIndex = Util.RandomByWeight(m_ListWeight.ToArray());
                        m_ListWeight.Remove(m_CurrChildIndex);

                        if (state == BehaviorTreeState.Success)
                        {
                            m_State = BehaviorTreeState.Success;
                            return;
                        }
                    }
                }
                else
                {
                    m_CurrChildIndex = Util.RandomByWeight(m_ListWeight.ToArray());
                    m_ListWeight.Remove(m_CurrChildIndex);
                }
            }

            if (m_ListWeight.Count < 1)
            {
                Reset();
                m_State = BehaviorTreeState.Failure;
            }
        }

        public override bool CanExcute()
        {
            return (m_ListWeight.Count > 0 || m_Weights.Length > 0) && m_State != BehaviorTreeState.Success;
        }

        public override void Reset()
        {
            base.Reset();
            m_ListWeight.Clear();
            m_LastChildIndex = -1;
        }

        private int m_CurrChildIndex;
        private int m_LastChildIndex;
        private int[] m_Weights;
        private List<int> m_ListWeight = null;
        private Regex m_Regex = new Regex(@"(Weight:)(\[)([^\[\]]+)(\])");
    }
}