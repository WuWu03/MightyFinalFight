using System;

namespace GameFrameWork.Fsm
{
    public abstract class BaseFsm
    {
        public BaseFsm(System.Object owner, string name, params BaseFsmState[] states)
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

        public System.Object owner
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
        public abstract void ShutDown();

        private string m_Name;
        private System.Object m_Owner;
    }
}
