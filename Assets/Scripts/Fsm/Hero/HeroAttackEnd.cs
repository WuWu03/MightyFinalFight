using GameFrameWork.Fsm;
using UnityEngine;

public class HeroAttackEnd : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.AttackEnd, 1);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            fsm.SetDefaultState<RoleIdle>();
            fsm.ChangeDefaultState();
        }
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner.StopAnimation(AnimName.Catch);
    }

    private BaseHero m_Owner = null;
}