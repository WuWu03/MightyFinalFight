using GameFrameWork.Fsm;

public class HeroPickUp : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(false);
        m_Owner.SetCanBeHit(false);
        m_Owner.SetCanJump(false);
        m_Owner.SetCanMove(false);
        m_Owner.SetCanSkill(false);

        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos, true);
        m_Owner.PlayAnimation(AnimName.PickUp, 1);
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
        m_Owner.StopAnimation(AnimName.PickUp);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}
