using GameFrameWork.Fsm;
using UnityEngine;

public class RoleDefense : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Timer = -1f;
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Defense);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
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

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Timer = -1f;
        m_Owner.StopAnimation(AnimName.Defense);
    }

    protected override void OnRelease(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private float m_Timer = -1f;
    private BaseRole m_Owner = null;
}