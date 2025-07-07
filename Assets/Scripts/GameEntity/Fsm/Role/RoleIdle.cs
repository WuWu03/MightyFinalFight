using GameFrameWork.FSM;
using UnityEngine;

public class RoleIdle : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos, true);

        if (m_Owner.objectType == ObjectType.Player && (m_Owner as BaseHero).weapon != null)
        {
            m_Owner.PlayAnimation(AnimName.Idle_Weapon);
        }
        else
        {
            m_Owner.PlayAnimation(AnimName.Idle);
        }
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
        m_Owner.StopAnimation(AnimName.Idle_Weapon);
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}