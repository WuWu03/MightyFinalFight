using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Fsm
{
    public class FsmMachine : BaseFsm
    {
        public FsmMachine(System.Object owner, string name, params BaseFsmState[] states) : base(owner, name, states)
        {
            m_CurrentState = null;
            m_CurrentStateTime = 0;
            m_DicStates = new Dictionary<Type, BaseFsmState>();
            m_IsDestroyed = false;

            if (owner == null)
            {
                throw new Exception("Fsm owner (type [" + owner.GetType().Name + "] is invalid!");
            }

            if (states != null && states.Length > 0)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] == null)
                    {
                        throw new Exception("Fsm state is invalid.");
                    }

                    states[i].Init(this);
                    m_DicStates.Add(states[i].GetType(), states[i]);
                }
            }
        }

        public override int stateCount
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
                return Mathf.Max(0, Time.time - m_CurrentStateTime);
            }
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
            m_CurrentStateTime = Time.time;
            m_CurrentState = fsmState;
            fsmState.Enter(this);
        }

        public override void AddState<T>()
        {
            if(!m_DicStates.ContainsKey(typeof(T)))
            {
                T state = new T();
                state.Init(this);
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

        public override void SetStateData<T>(BaseEventArgs stateData)
        {
            BaseFsmState state = GetState<T>();

            if (state == null)
            {
                throw new Exception(StringUtil.Format("Fsm [", typeof(T).Name, "] state is invalid or destroyed."));
            }

            state.SetStateData(stateData);
        }

        public override void ChangeState<T>(BaseEventArgs stateData = null)
        {
            if (!isRunning)
            {
                throw new Exception("Fsm is not in running please use Start() to run it first.");
            }

            if (m_CurrentState == null)
            {
                throw new Exception("Fsm current state is invalid or destroyed.");
            }

            if (m_CurrentState.GetType().Equals(typeof(T)))
            {
                if(stateData != null)
                {
                    m_CurrentState.SetStateData(stateData);
                }

                return;
            }

            BaseFsmState state = GetState<T>();

            if (state == null)
            {
                throw new Exception(StringUtil.Format("Fsm [", typeof(T).Name, "] state is invalid or destroyed."));
            }

            if(stateData != null)
            {
                state.SetStateData(stateData);
            }

            m_CurrentStateTime = Time.time;
            m_CurrentState.Exit(this, false);
            m_CurrentState = state;
            m_CurrentState.Enter(this);
        }

        public override void ChangeDefaultState()
        {
            if (!isRunning)
            {
                throw new Exception("Fsm is not in running please use Start() to run it first.");
            }

            if (m_CurrentState == null)
            {
                throw new Exception("Fsm current state is invalid or destroyed.");
            }

            if (m_DefaultState == null)
            {
                throw new Exception("Fsm default state is invalid or destroyed.");
            }

            if (m_CurrentState == m_DefaultState)
            {
                return;
            }

            m_CurrentStateTime = 0f;
            m_CurrentState.Exit(this, false);
            m_CurrentState = m_DefaultState;
            m_CurrentState.Enter(this);
        }

        public override T GetState<T>()
        {
            if (!m_DicStates.TryGetValue(typeof(T), out BaseFsmState result))
            {
                return null;
            }

            return result as T;
        }

        public override BaseFsmState[] GetAllStates()
        {
            BaseFsmState[] results = new BaseFsmState[m_DicStates.Count];
            m_DicStates.Values.CopyTo(results, 0);
            return results;
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

            m_CurrentState.Update(this, deltaTime, unscaleDeltaTime);
        }

        public override void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime)
        {
            if (m_CurrentState == null)
            {
                return;
            }

            m_CurrentState.FixedUpdate(this, fixedDeltaTime, fixedUnscaledDeltaTime);
        }

        public override void Release()
        {
            foreach (KeyValuePair<Type, BaseFsmState> kvp in m_DicStates)
            {
                kvp.Value.Release(this);
            }

            m_DicStates.Clear();
            m_CurrentState = null;
            m_DefaultState = null;
            m_CurrentStateTime = 0f;
        }

        public override void ShutDown()
        {
            if(m_IsDestroyed)
            {
                return;
            }

            foreach(KeyValuePair<Type, BaseFsmState> kvp in m_DicStates)
            {
                kvp.Value.Release(this);
            }

            m_DicStates.Clear();
            m_DicStates = null;
            m_CurrentState = null;
            m_DefaultState = null;
            m_CurrentStateTime = 0f;
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

        private Dictionary<Type, BaseFsmState> m_DicStates;
        private BaseFsmState m_CurrentState;
        private BaseFsmState m_DefaultState;
        private float m_CurrentStateTime;
        private bool m_IsDestroyed;
    }
}