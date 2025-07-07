using UnityEngine;

namespace GameFrameWork.FSM
{
	public abstract class BaseFsmState
	{
		public void Init(FiniteStateMachine fsm)
		{
			OnInit(fsm);
		}

		public void Enter(FiniteStateMachine fsm)
		{
			OnEnter(fsm);
		}

		public void Update(FiniteStateMachine fsm, float deltaTime, float unscaledDeltaTime)
		{
			OnUpdate(fsm, deltaTime, unscaledDeltaTime);
		}

		public void LateUpdate(FiniteStateMachine fsm, float deltaTime, float unscaledDeltaTime)
		{
			OnLateUpdate(fsm, deltaTime, unscaledDeltaTime);
		}

		public void FixedUpdate(FiniteStateMachine fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
		{
			OnFixedUpdate(fsm, fixedDeltaTime, fixedUnscaledDeltaTime);
		}

		public void Exit(FiniteStateMachine fsm, bool isShutdown)
		{
			OnExit(fsm, isShutdown);
		}

		public void Release(FiniteStateMachine fsm)
		{
			OnRelease(fsm);
		}

		public void SetStateData(BaseEventArgs stateData)
		{
			OnSetStateData(stateData);
		}

		protected virtual void OnInit(FiniteStateMachine fsm) { }
		protected abstract void OnEnter(FiniteStateMachine fsm);
		protected virtual void OnUpdate(FiniteStateMachine fsm, float deltaTime, float unscaledDeltaTime) { }
		protected virtual void OnLateUpdate(FiniteStateMachine fsm, float deltaTime, float unscaledDeltaTime) { }
		protected virtual void OnFixedUpdate(FiniteStateMachine fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime) { }
		protected abstract void OnExit(FiniteStateMachine fsm, bool isShutdown);
		protected virtual void OnRelease(FiniteStateMachine fsm) { }

		protected virtual void OnSetStateData(BaseEventArgs stateData) { }

		protected void ChangeState<T>(FiniteStateMachine fsm, BaseEventArgs stateData = null) where T : BaseFsmState
		{
			if (fsm == null)
			{
				return;
			}

			fsm.ChangeState<T>(stateData);
		}
	}
}
