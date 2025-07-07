using GameFrameWork.FSM;

public class HeroCatch : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos(m_Owner.pos, m_Owner.posZ);
        m_Owner.PlayAnimation(AnimName.Catch, 1);
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Catch);
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private BaseHero m_Owner = null;
}