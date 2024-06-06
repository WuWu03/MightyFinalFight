namespace GameFrameWork.Fsm
{
    public abstract class BaseFsmState
	{
		public void Init(BaseFsm fsm)
		{
			OnInit(fsm);
		}

		public void Enter(BaseFsm fsm)
		{
			OnEnter(fsm);
		}

		public void Update(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
		{
			OnUpdate(fsm, deltaTime, unscaleDeltaTime);
		}

		public void FixedUpdate(BaseFsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
		{
			OnFixedUpdate(fsm, fixedDeltaTime, fixedUnscaledDeltaTime);
		}

		public void Exit(BaseFsm fsm, bool isShutdown)
		{
			OnExit(fsm, isShutdown);
		}

		public void Release(BaseFsm fsm)
		{
			OnRelease(fsm);
		}

		public void SetStateData(BaseEventArgs stateData)
		{
			OnSetStateData(stateData);
        }

		protected virtual void OnInit(BaseFsm fsm) { }
		protected abstract void OnEnter(BaseFsm fsm);
		protected virtual void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime) { }
		protected virtual void OnFixedUpdate(BaseFsm fsm, float fixedDeltaTime, float unscaleDeltaTime) { }
		protected abstract void OnExit(BaseFsm fsm, bool isShutdown);
		protected virtual void OnRelease(BaseFsm fsm) { }

		protected virtual void OnSetStateData(BaseEventArgs stateData) { }

		protected void ChangeState<T>(BaseFsm fsm, BaseEventArgs stateData = null) where T : BaseFsmState
		{
			if (fsm == null)
			{
				return;
			}

			fsm.ChangeState<T>(stateData);
		}
	}
}
