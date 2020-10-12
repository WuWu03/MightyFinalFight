using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Fsm
{
    public abstract class BaseFsm
    {
        public BaseFsm(System.Object owner,string name) 
        {
            this.m_Owner = owner;
            this.Name = name;
        }

        public string Name
        {
            get { return m_Name; }
            protected set { this.m_Name = value; }
        }

        public System.Object Owner
        {
            get { return m_Owner; }
            protected set { this.m_Owner = value; }
        }

        public Type OwnerType
        {
            get
            {
                return m_Owner.GetType();
            }
        }

        public abstract int FsmStateCount { get; }
        public abstract bool IsRunning { get; }
        public abstract bool IsDestroy { get; }
        public abstract Type CurrStateType { get; }
        public abstract BaseFsmState CurrState { get; }
        public abstract float CurrStateTime { get; }
        public abstract void Start<T>() where T:BaseFsmState;
        public abstract void AddState<T>() where T : BaseFsmState, new();
        public abstract void RemoveState<T>() where T : BaseFsmState;
        public abstract void ChangeState<T>(bool isForce) where T : BaseFsmState;
        public abstract bool HasState<T>() where T : BaseFsmState;
        public abstract T GetState<T>() where T : BaseFsmState;
        public abstract void SetDefaultState<T>() where T : BaseFsmState;
        public abstract BaseFsmState[] GetAllStates();
        public abstract void Update(float deltaTime, float unscaleDeltaTime);
        public abstract void ShutDown();

        private string m_Name;
        private System.Object m_Owner;
    }
}
