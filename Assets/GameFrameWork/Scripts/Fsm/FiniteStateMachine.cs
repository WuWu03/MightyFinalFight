using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.FSM
{
    public class FiniteStateMachine
    {
        public FiniteStateMachine(object owner, string name, params BaseFsmState[] states)
        {
            m_Owner = owner;
            m_Name = name;
            m_CurrentState = null;
            m_CurrentStateTime = 0;
            m_DicStates = new Dictionary<Type, BaseFsmState>();
            m_IsDestroyed = false;

            if (owner == null)
            {
                Log.LogError("状态机持有者不存在，请检查");
            }

            if (states != null && states.Length > 0)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] == null)
                    {
                        Log.LogError("状态不存在，请检查状态列表");
                    }

                    states[i].Init(this);
                    m_DicStates.Add(states[i].GetType(), states[i]);
                }
            }
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

        public int stateCount
        {
            get
            {
                return m_DicStates.Count;
            }
        }

        public bool isRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        public bool isDestroy
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        public BaseFsmState currState
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

        public void Start<T>() where T : BaseFsmState
        {
            if (isRunning)
            {
                Log.LogError("有限状态机已经启动，不要重复启动");
            }

            BaseFsmState fsmState = this.GetState<T>();

            if (fsmState == null)
            {
                Log.LogError("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态");
            }

            m_DefaultState = fsmState;
            m_CurrentStateTime = Time.time;
            m_CurrentState = fsmState;
            fsmState.Enter(this);
        }

        public void Pause()
        {
            m_IsPaused = true;
        }

        public void Resume()
        {
            m_IsPaused = false;
        }

        public void AddState<T>() where T : BaseFsmState, new()
        {
            if (!m_DicStates.ContainsKey(typeof(T)))
            {
                T state = new T();
                state.Init(this);
                m_DicStates.Add(typeof(T), state);
            }
        }

        public void RemoveState<T>() where T : BaseFsmState
        {
            if (m_DicStates.ContainsKey(typeof(T)))
            {
                m_DicStates.Remove(typeof(T));
            }
        }

        public void SetStateData<T>(BaseEventArgs stateData) where T : BaseFsmState
        {
            BaseFsmState state = GetState<T>();

            if (state == null)
            {
                Log.LogError("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态");
            }

            state.SetStateData(stateData);
        }

        public void ChangeState<T>(BaseEventArgs stateData = null) where T : BaseFsmState
        {
            if (!isRunning)
            {
                Log.LogError("有限状态机没有启动，调用Start方法启动");
            }

            if (m_CurrentState.GetType().Equals(typeof(T)))
            {
                if (stateData != null)
                {
                    m_CurrentState.SetStateData(stateData);
                }

                return;
            }

            BaseFsmState state = GetState<T>();

            if (state == null)
            {
                Log.LogError("[", typeof(T).Name, "] 状态不存在，调用AddState方法添加该状态");
            }

            if (stateData != null)
            {
                state.SetStateData(stateData);
            }

            m_CurrentStateTime = Time.time;
            m_CurrentState.Exit(this, false);
            m_CurrentState = state;
            m_CurrentState.Enter(this);
        }

        public void SetDefaultState<T>() where T : BaseFsmState
        {
            BaseFsmState state = this.GetState<T>();

            if (state == null)
            {
                Log.LogError("状态 [", typeof(T).Name, "] 不存在，调用AddState方法添加该状态");
            }

            m_DefaultState = state;
        }

        public void ChangeDefaultState()
        {
            if (m_CurrentState == null)
            {
                Log.LogError("有限状态机没有启动，调用Start方法启动");
            }

            if (m_DefaultState == null)
            {
                Log.LogError("默认状态不存在，调用SetDefaultState方法设置默认状态");
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

        public T GetState<T>() where T : BaseFsmState
        {
            if (!m_DicStates.TryGetValue(typeof(T), out BaseFsmState result))
            {
                return null;
            }

            return result as T;
        }

        public BaseFsmState[] GetAllStates()
        {
            BaseFsmState[] results = new BaseFsmState[m_DicStates.Count];
            m_DicStates.Values.CopyTo(results, 0);
            return results;
        }

        public bool HasState<T>() where T : BaseFsmState
        {
            return m_DicStates.ContainsKey(typeof(T));
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
            foreach (KeyValuePair<Type, BaseFsmState> kvp in m_DicStates)
            {
                kvp.Value.Release(this);
            }

            m_DicStates.Clear();
            m_CurrentState = null;
            m_DefaultState = null;
            m_CurrentStateTime = 0f;
        }

        public void ShutDown()
        {
            if (m_IsDestroyed)
            {
                return;
            }

            foreach (KeyValuePair<Type, BaseFsmState> kvp in m_DicStates)
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

        private readonly string m_Name = string.Empty;
        private readonly object m_Owner = null;
        private float m_CurrentStateTime;
        private bool m_IsPaused = false;
        private bool m_IsDestroyed = false;

        private Dictionary<Type, BaseFsmState> m_DicStates;
        private BaseFsmState m_CurrentState;
        private BaseFsmState m_DefaultState;
    }
}