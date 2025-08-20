using UnityEngine;

namespace GameFrameWork.Fsm
{
    public abstract class BaseFsmState
    {
        public void Init(Fsm fsm)
        {
            OnInit(fsm);
        }

        public void Enter(Fsm fsm)
        {
            OnEnter(fsm);
        }

        public void Update(Fsm fsm, float deltaTime, float unscaledDeltaTime)
        {
            OnUpdate(fsm, deltaTime, unscaledDeltaTime);
        }

        public void LateUpdate(Fsm fsm, float deltaTime, float unscaledDeltaTime)
        {
            OnLateUpdate(fsm, deltaTime, unscaledDeltaTime);
        }

        public void FixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
        {
            OnFixedUpdate(fsm, fixedDeltaTime, fixedUnscaledDeltaTime);
        }

        public void Exit(Fsm fsm, bool isShutdown)
        {
            OnExit(fsm, isShutdown);
        }

        public void Release(Fsm fsm)
        {
            OnRelease(fsm);
        }

        public void SetStateData(Fsm fsm, BaseEventArgs stateData)
        {
            OnSetStateData(fsm, stateData);
        }

        protected virtual void OnInit(Fsm fsm) { }
        protected abstract void OnEnter(Fsm fsm);
        protected virtual void OnUpdate(Fsm fsm, float deltaTime, float unscaledDeltaTime) { }
        protected virtual void OnLateUpdate(Fsm fsm, float deltaTime, float unscaledDeltaTime) { }
        protected virtual void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime) { }
        protected abstract void OnExit(Fsm fsm, bool isShutdown);
        protected virtual void OnRelease(Fsm fsm) { }

        protected virtual void OnSetStateData(Fsm fsm, BaseEventArgs stateData) { }

        protected void ChangeState<T>(Fsm fsm, BaseEventArgs stateData = null) where T : BaseFsmState
        {
            if (fsm == null)
            {
                return;
            }

            fsm.ChangeState<T>(stateData);
        }
    }
}
