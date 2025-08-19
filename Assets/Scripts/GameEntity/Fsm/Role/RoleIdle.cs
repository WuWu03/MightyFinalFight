using GameFrameWork.Fsm;
using UnityEngine;

public class RoleIdle : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(true);
        m_Owner.SetCanBeHit(true);
        m_Owner.SetCanJump(true);
        m_Owner.SetCanMove(true);
        m_Owner.SetCanSkill(true);
        m_Owner.SetCanBeCatch(true);
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

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
        m_Owner.StopAnimation(AnimName.Idle_Weapon);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}