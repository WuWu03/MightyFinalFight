using GameFrameWork.Fsm;

public class BarrelIdle : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.Idle);
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private Barrel m_Owner = null;
}
