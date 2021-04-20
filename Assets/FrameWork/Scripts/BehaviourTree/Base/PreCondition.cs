using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace GameFrameWork.BehaviourTree
{
    public abstract class PreCondition : Node
    {
        public PreCondition(string name, string args, object owner) : base(name, args, owner)
        {
            if (!string.IsNullOrEmpty(args))
            {
                Match m = m_Regex.Match(args);
                if (m.Success) m_IsNot = bool.Parse(m.Groups[2].Value);
            }
        }

        public override void AddChild(Node node)
        {
            throw new System.Exception("Can not add child to a leaf which type is <PreCondition>");
        }

        public override void AddPreCondition(Node node)
        {
            throw new System.Exception("Can not add precondition to a leaf which type is <PreCondition>");
        }

        public override bool CheckPreCondition()
        {
            if (m_IsNot)
            {
                return !OnCheckPreCondition();
            }

            return OnCheckPreCondition();
        }

        protected override void OnEnter() { }
        protected override void OnUpdate(float deltaTime) { }
        protected override void OnDestroy() { }
        protected abstract bool OnCheckPreCondition();

        private bool m_IsNot = false;
        private Regex m_Regex = new Regex(@"(TheNot:)(true|false)");
    }
}