using GameFrameWork.Fsm;

public class HeroAttackEnd : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.AttackEnd, 1);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsCurrAnimationComplete())
        {
            fsm.SetDefaultState<RoleIdle>();
            fsm.ChangeDefaultState();
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.AttackEnd);
        m_Owner.StopAnimation(AnimName.Catch);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private BaseHero m_Owner = null;
}