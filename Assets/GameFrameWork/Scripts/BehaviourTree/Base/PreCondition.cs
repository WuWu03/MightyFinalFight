using System.Text.RegularExpressions;


namespace GameFrameWork.BehaviourTree
{
    public abstract class PreCondition : Node
    {
        public PreCondition(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                Match m = m_Regex.Match(args);
                if (m.Success)
                {
                    m_IsNot = bool.Parse(m.Groups[2].Value);
                }
            }
        }

        public override bool CheckPreCondition()
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
        private Regex m_Regex = new Regex(@"(TheNot:)(true|false)");
    }
}