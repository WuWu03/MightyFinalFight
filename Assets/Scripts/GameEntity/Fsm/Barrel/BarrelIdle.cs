using WuWuFramework.Fsm;

public class BarrelIdle : FsmState
{
    private Barrel m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.Idle);
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }
}
