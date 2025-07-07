using GameFrameWork.FSM;
using UnityEngine;

public class HeroAttackEnd : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.AttackEnd, 1);
    }

    protected override void OnUpdate(FiniteStateMachine fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            fsm.SetDefaultState<RoleIdle>();
            fsm.ChangeDefaultState();
        }
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.AttackEnd);
        m_Owner.StopAnimation(AnimName.Catch);
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private BaseHero m_Owner = null;
}