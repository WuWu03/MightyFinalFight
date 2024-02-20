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

        public void FixedUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
        {
            OnFixedUpdate(fsm, deltaTime, unscaleDeltaTime);
        }

		public void Exit(BaseFsm fsm, bool isShutdown)
		{
			OnExit(fsm, isShutdown);
		}

		public void Destroy(BaseFsm fsm)
		{
			OnDestroy(fsm);
		}

        protected virtual void OnInit(BaseFsm fsm) { }
		protected abstract void OnEnter(BaseFsm fsm);
        protected virtual void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime) { }
        protected virtual void OnFixedUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime) { }
        protected abstract void OnExit(BaseFsm fsm, bool isShutdown);
		protected virtual void OnDestroy(BaseFsm fsm) { }
		protected void ChangeState<T>(BaseFsm fsm,bool isForce = false) where T : BaseFsmState
		{
			if(fsm != null)
			{
				fsm.ChangeState<T>(isForce);
			}
		}
	}
}
