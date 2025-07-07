using GameFrameWork.FSM;
using UnityEngine;

public class RoleDefense : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Timer = -1f;
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Defense);
    }

    protected override void OnUpdate(FiniteStateMachine fsm, float deltaTime, float unscaleDeltaTime)
    {
        if(m_Timer <= 0f)
        {
            m_Timer = Time.time;
        }

        if (Time.time - m_Timer > 0.5f)
        {
            ChangeState<RoleIdle>(fsm);
        }
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Timer = -1f;
        m_Owner.StopAnimation(AnimName.Defense);
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private float m_Timer = -1f;
    private BaseRole m_Owner = null;
}