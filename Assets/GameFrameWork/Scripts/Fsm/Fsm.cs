using System;
using System.Collections.Generic;
using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.Fsm
{
    public class Fsm : IReference
    {
        private readonly Dictionary<Type, FsmState> m_FsmStates;
        private FsmState m_CurrentState;
        private FsmState m_DefaultState;
        private string m_Name = string.Empty;
        private object m_Owner;
        private float m_CurrentStateTime;
        private bool m_IsPaused;
        
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

        public int stateCount
        {
            get
            {
                return m_FsmStates.Count;
            }
        }

        public bool isRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        public FsmState currState
        {
            get
            {
                return m_CurrentState;
            }
        }

        public Type currStateType
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

        public float currStateTime
        {
            get
            {
                return Mathf.Max(0, Time.time - m_CurrentStateTime);
            }
        }

        public Fsm()
        {
            m_FsmStates = new();
        }

        public static Fsm Create(object owner, string name)
        {
            Fsm fsm = ReferencePool.Acquire<Fsm>();
            fsm.m_Owner = owner;
            fsm.m_Name = name;
            fsm.m_CurrentState = null;
            fsm.m_CurrentStateTime = 0;
            return fsm;
        }
        /// <summary>
        /// 以指定状态运行状态机
        /// </summary>
        public void Start<T>() where T : FsmState
        {
            FsmState fsmState = this.GetState<T>();

            if (fsmState == null)
            {
                Log.LogError("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态");
            }

            Start(fsmState);
        }

        /// <summary>
        /// 以默认状态运行状态机
        /// </summary>
        public void Start()
        {
            Start(m_DefaultState);
        }

        public void Pause()
        {
            m_IsPaused = true;
        }

        public void Resume()
        {
            m_IsPaused = false;
        }

        public void AddState<T>() where T : FsmState, new()
        {
            if (!m_FsmStates.ContainsKey(typeof(T)))
            {
                T state = new();
                state.Init(this);
                m_FsmStates.Add(typeof(T), state);
            }
        }

        public void RemoveState<T>() where T : FsmState
        {
            if (m_FsmStates.ContainsKey(typeof(T)))
            {
                m_FsmStates.Remove(typeof(T));
            }
        }

        public void SetStateData<T>(FsmStateArg stateData) where T : FsmState
        {
            FsmState state = GetState<T>();

            if (state == null)
            {
                throw new Exception(StringUtil.Append("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态"));
            }

            state.SetStateData(this, stateData);
        }

        public void ChangeState<T>(FsmStateArg stateData = null) where T : FsmState
        {
            if (!isRunning)
            {
                throw new Exception("有限状态机没有启动，调用Start方法启动");
            }

            if (m_CurrentState is T)
            {
                if (stateData != null)
                {
                    m_CurrentState.SetStateData(this, stateData);
                }

                return;
            }

            FsmState state = GetState<T>();

            if (state == null)
            {
                throw new Exception(StringUtil.Append("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态"));
            }

            if (stateData != null)
            {
                state.SetStateData(this, stateData);
            }

            m_CurrentStateTime = Time.time;
            m_CurrentState.Exit(this, false);
            m_CurrentState = state;
            m_CurrentState.Enter(this);
        }

        public void SetDefaultState<T>() where T : FsmState
        {
            FsmState state = this.GetState<T>();

            if (state == null)
            {
                throw new Exception(StringUtil.Append("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态"));
            }

            m_DefaultState = state;
        }

        public void ChangeDefaultState()
        {
            if (m_CurrentState == null)
            {
                throw new Exception("有限状态机没有启动，调用Start方法启动");
            }

            if (m_DefaultState == null)
            {
                throw new Exception("默认状态不存在，调用SetDefaultState方法设置默认状态");
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

        public T GetState<T>() where T : FsmState
        {
            if (!m_FsmStates.TryGetValue(typeof(T), out FsmState result))
            {
                return null;
            }

            return result as T;
        }

        public FsmState[] GetAllStates()
        {
            FsmState[] results = new FsmState[m_FsmStates.Count];
            m_FsmStates.Values.CopyTo(results, 0);
            return results;
        }

        public bool HasState<T>() where T : FsmState
        {
            return m_FsmStates.ContainsKey(typeof(T));
        }

        public bool HasDefaultState()
        {
            return m_DefaultState != null;
        }

        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (m_CurrentState == null || m_IsPaused)
            {
                return;
            }

            m_CurrentState.Update(this, deltaTime, unscaledDeltaTime);
        }

        public void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_CurrentState == null || m_IsPaused)
            {
                return;
            }
            
            m_CurrentState.LateUpdate(this, deltaTime, unscaledDeltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime)
        {
            if (m_CurrentState == null || m_IsPaused)
            {
                return;
            }

            m_CurrentState.FixedUpdate(this, fixedDeltaTime, fixedUnscaledDeltaTime);
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            foreach (KeyValuePair<Type, FsmState> kvp in m_FsmStates)
            {
                kvp.Value.Release(this);
            }

            m_FsmStates.Clear();
            m_Name = string.Empty;
            m_Owner = null;
            m_CurrentStateTime = 0f;
            m_IsPaused = false;
            m_CurrentState = null;
            m_DefaultState = null;
        }

        private void Start(FsmState fsmState)
        {
            if (isRunning)
            {
                throw new Exception("有限状态机已经启动，不要重复启动");
            }

            if (fsmState == null)
            {
                throw new Exception("默认状态不存在，调用AddState方法添加该状态");
            }

            m_CurrentStateTime = Time.time;
            m_CurrentState = fsmState;
            m_DefaultState ??= fsmState;
            fsmState.Enter(this);
        }
    }
}