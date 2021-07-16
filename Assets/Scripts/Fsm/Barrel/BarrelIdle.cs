using GameFrameWork.Fsm;

public class BarrelIdle : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as Barrel;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.BarrelAnimator.animation.Play(AnimName.Idle, 0);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.BarrelAnimator.animation.Stop(AnimName.Idle);
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    public override void SetParam(object[] args)
    {

    }

    private Barrel m_Owner = null;
}
