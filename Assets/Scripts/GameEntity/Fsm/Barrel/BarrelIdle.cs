using GameFrameWork.FSM;

public class BarrelIdle : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.PlayAnimation(AnimName.Idle);
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private Barrel m_Owner = null;
}
