using WuWuFramework.Fsm;
using UnityEngine;

public class RoleDefense : FsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        m_Timer = -1f;
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Defense);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
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

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Timer = -1f;
        m_Owner.StopAnimation(AnimName.Defense);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private float m_Timer = -1f;
    private BaseRole m_Owner = null;
}