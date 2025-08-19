using GameFrameWork.Fsm;

public class HeroCatch : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(true);
        m_Owner.SetCanBeHit(true);
        m_Owner.SetCanJump(true);
        m_Owner.SetCanMove(m_Owner.isCatchContrl);
        m_Owner.SetCanSkill(true);

        m_Owner.ResetRigidbody();
        m_Owner.SetPos(m_Owner.pos, m_Owner.posZ);
        m_Owner.PlayAnimation(AnimName.Catch, 1);
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Catch);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private BaseHero m_Owner = null;
}