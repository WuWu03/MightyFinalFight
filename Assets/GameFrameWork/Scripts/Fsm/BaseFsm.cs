using System;

namespace GameFrameWork.Fsm
{
    public abstract class BaseFsm
    {
        public BaseFsm(object owner, string name)
        {
            m_Owner = owner;
            m_Name = name;
        }

        public string name
        {
            get 
            { 
                return m_Name;
            }
        }

        public object owner
        {
            get
            {
                return m_Owner;
            }
        }

        public Type ownerType
        {
            get
            {
                return m_Owner.GetType();
            }
        }

        public abstract int stateCount { get; }
        public abstract bool isRunning { get; }
        public abstract bool isDestroy { get; }
        public abstract Type currStateType { get; }
        public abstract BaseFsmState currState { get; }
        public abstract float currStateTime { get; }

        public abstract void Start<T>() where T:BaseFsmState;
        public abstract void Pause();
        public abstract void Resume();
        public abstract void AddState<T>() where T : BaseFsmState, new();
        public abstract void RemoveState<T>() where T : BaseFsmState;
        public abstract void SetStateData<T>(BaseEventArgs stateData) where T : BaseFsmState;
        public abstract void ChangeState<T>(BaseEventArgs stateData) where T : BaseFsmState;
        public abstract bool HasState<T>() where T : BaseFsmState;
        public abstract T GetState<T>() where T : BaseFsmState;
        public abstract void SetDefaultState<T>() where T : BaseFsmState;
        public abstract void ChangeDefaultState();
        public abstract BaseFsmState[] GetAllStates();
        public abstract void Update(float deltaTime, float unscaleDeltaTime);
        public abstract void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime);
        public abstract void Release();
        public abstract void ShutDown();

        private readonly string m_Name = string.Empty;
        private readonly object m_Owner = null;
    }
}
