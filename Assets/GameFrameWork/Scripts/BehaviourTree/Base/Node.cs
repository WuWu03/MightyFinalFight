namespace GameFrameWork.BehaviourTree
{
    public enum BehaviourTreeState
    {
        None = 0,
        Success = 1,
        Running = 2,
        Failure = 3,
    }

    public abstract class Node
    {
        public int id
        {
            get
            {
                return m_ID;
            }
        }

        public object owner
        {
            get
            {
                return m_Owner;
            }
        }

        public int priority
        {
            get
            {
                return m_Priority;
            }
        }

        public string args
        {
            get
            {
                return m_Args;
            }
        }

        public Node(int id, object owner, int priority, string args)
        {
            m_ID = id;
            m_Owner = owner;
            m_Priority = priority;
            m_Args = args;
        }

        public void Start()
        {
            OnStart();
        }

        public void Enter()
        {
            if (m_HasEnter)
            {
                return;
            }

            m_HasEnter = true;
            OnEnter();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            OnLateUpdate(deltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            OnFixedUpdate(fixedDeltaTime);
        }

        public void Destroy()
        {
            OnDestroy();
            m_Args = string.Empty;
            m_Owner = null;
        }

        public void Reset()
        {
            m_HasEnter = false;
            OnReset();
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnter() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnLateUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }
        protected virtual void OnDestroy() { }
        protected virtual void OnReset() { }

        private int m_ID = 0;
        private object m_Owner = null;
        private int m_Priority = 0;
        private string m_Args = string.Empty;
        private bool m_HasEnter = false;
    }
}
