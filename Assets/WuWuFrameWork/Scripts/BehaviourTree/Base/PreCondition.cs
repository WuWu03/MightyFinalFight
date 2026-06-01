using System.Text.RegularExpressions;


namespace WuWuFramework.BehaviourTree
{
    public abstract class PreCondition : Node
    {
        public bool isAndCondition
        {
            get
            {
                return m_IsAndCondition;
            }
        }

        public PreCondition(int id, object owner, int priority,bool isAndCondition, string args) : base(id, owner, priority, args)
        {
            m_IsAndCondition = isAndCondition;
            m_Regex = new(@"(TheNot:)(true|false)");

            if (!string.IsNullOrEmpty(args))
            {
                Match m = m_Regex.Match(args);
                if (m.Success)
                {
                    m_IsNot = bool.Parse(m.Groups[2].Value);
                }
            }
        }

        public bool CheckPreCondition()
        {
            if (m_IsNot)
            {
                return !OnCheckPreCondition();
            }

            return OnCheckPreCondition();
        }

        protected override void OnStart() { }
        protected override void OnEnter() { }
        protected override void OnUpdate(float deltaTime) { }
        protected override void OnDestroy() { }
        protected abstract bool OnCheckPreCondition();

        private bool m_IsNot = false;
        private bool m_IsAndCondition = false;
        private Regex m_Regex = null;
    }
}