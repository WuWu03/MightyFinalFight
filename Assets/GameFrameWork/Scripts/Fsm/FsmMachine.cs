using System;
using System.Collections.Generic;

namespace GameFrameWork.Fsm
{
    public class FsmMachine : BaseFsm
    {
        public FsmMachine(System.Object owner,string name):base(owner,name)
        {
            m_CurrentState = null;
            m_CurrentStateTime = 0;
            m_DicStates = new Dictionary<Type, BaseFsmState>();
            m_IsDestroyed = true;
        }
        public override int fsmStateCount
        {
            get
            {
                return m_DicStates.Count;
            }
        }

        public override bool isRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        public override bool isDestroy
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        public override BaseFsmState currState
        {
            get
            {
                return m_CurrentState;
            }
        }

        public override Type currStateType
        {
            get
            {
                if (m_CurrentState != null)
                {
                    return m_CurrentState.GetType();
                }

                return null;
            }
        }

        public override float currStateTime
        {
            get
            {
                return m_CurrentStateTime;
            }
        }
        
        public static FsmMachine Create(System.Object owner,string name,params BaseFsmState[] states)
        {
            if(owner == null)
            {
                throw new Exception("Fsm owner (type [" + owner.GetType().Name + "] is invalid!");
            }

            FsmMachine fsm = new FsmMachine(owner, name);
            fsm.name = name;
            fsm.m_IsDestroyed = false;

            if (states != null)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] == null)
                    {
                        throw new Exception("Fsm state is invalid.");
                    }

                    states[i].OnInit(fsm);
                    fsm.m_DicStates.Add(states[i].GetType(), states[i]);
                }
            }

            return fsm;
        }

        public override void Start<T>()
        {
            if (isRunning)
            {
                throw new Exception("Fsm is running.");
            }

            BaseFsmState fsmState = this.GetState<T>();

            if (fsmState == null)
            {
                throw new Exception("Fsm state is invalid.");
            }

            m_DefaultState = fsmState;
            m_CurrentStateTime = 0;
            m_CurrentState = fsmState;
            fsmState.OnEnter(this);
        }

        public override void AddState<T>()
        {
            if(!m_DicStates.ContainsKey(typeof(T)))
            {
                T state = new T();
                state.OnInit(this);
                m_DicStates.Add(typeof(T), state);
            }
        }

        public override void RemoveState<T>()
        {
            if (m_DicStates.ContainsKey(typeof(T)))
            {
                m_DicStates.Remove(typeof(T));
            }
        }

        public override void ChangeState<T>(bool isForce = false)
        {
            if (m_CurrentState == null)
            {
                throw new Exception("Fsm current state is invalid or destroyed.");
            }

            if (!isForce && m_CurrentState.GetType().Equals(typeof(T)))
            {
                return;
            }

            BaseFsmState state = GetState<T>();

            if (state == null)
            {
                throw new Exception("Fsm state is invalid.");
            }

            m_CurrentStateTime = 0f;
            m_CurrentState.OnExit(this, false);
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        public override void ChangeDefaultState()
        {
            if (m_CurrentState == null)
            {
                throw new Exception("Fsm is invalid or destroyed.");
            }

            if (m_DefaultState == null)
            {
                throw new Exception("Fsm is invalid or destroyed.");
            }

            m_CurrentStateTime = 0f;
            m_CurrentState.OnExit(this, false);
            m_CurrentState = m_DefaultState;
            m_CurrentState.OnEnter(this);
        }

        public override T GetState<T>()
        {
            if (!m_DicStates.TryGetValue(typeof(T), out BaseFsmState ret))
            {
                return null;
            }

            return ret as T;
        }

        public override BaseFsmState[] GetAllStates()
        {
            BaseFsmState[] ret = new BaseFsmState[m_DicStates.Count];
            m_DicStates.Values.CopyTo(ret, 0);
            return ret;
        }

        public override bool HasState<T>()
        {
            return m_DicStates.ContainsKey(typeof(T));
        }

        public override void Update(float deltaTime, float unscaleDeltaTime)
        {
            if (m_CurrentState == null) 
            {
                return;
            }

            m_CurrentStateTime += deltaTime;
            m_CurrentState.OnUpdate(this, deltaTime, unscaleDeltaTime);
        }

        public override void ShutDown()
        {
            m_CurrentState = null;
            m_CurrentStateTime = 0f;

            foreach(KeyValuePair<Type, BaseFsmState> kvp in m_DicStates)
            {
                kvp.Value.OnDestroy(this);
            }

            m_DicStates.Clear();
            m_IsDestroyed = true;
        }

        public override void SetDefaultState<T>()
        {
            BaseFsmState state = this.GetState<T>();

            if (state == null)
            {
                throw new Exception("Fsm state is invalid");
            }

            m_DefaultState = state;
        }

        private readonly Dictionary<Type, BaseFsmState> m_DicStates;
        private BaseFsmState m_CurrentState;
        private BaseFsmState m_DefaultState;
        private float m_CurrentStateTime;
        private bool m_IsDestroyed;
    }
}