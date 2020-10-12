namespace FrameWork.Fsm
{
	public abstract class BaseFsmState
	{
		public BaseFsmState(){}
		public abstract void OnInit(BaseFsm fsm);
		public abstract void OnEnter(BaseFsm fsm);
		public abstract void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime);
		public abstract void OnExit(BaseFsm fsm, bool isShutdown);
		public abstract void OnDestroy(BaseFsm fsm);
		protected void ChangeState<T>(BaseFsm fsm,bool isForce = false) where T : BaseFsmState
		{
			if(fsm != null)
			{
				fsm.ChangeState<T>(isForce);
			}
		}
	}
}
