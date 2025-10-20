using GameFrameWork.Fsm;
using UnityEngine;

public class RoleSkill : FsmState
{
    private bool m_CanMove;
    private bool m_CanChangeDir;
    private float m_Dir;
    private BaseRole m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(m_Owner.isAttack);
        m_Owner.SetCanMove(true);
        m_Owner.SetCanJump(false);
        m_Owner.SetCanBeCatch(false);
        m_Owner.SetCanSkill(false);
    }

    protected override void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }

        if (m_CanMove)
        {
            Vector3 ownerPos = m_Owner.transform.localPosition + fixedDeltaTime * m_Owner.entityAttribute.moveSpeed * m_Dir * Vector3.right;
            m_Owner.SetPos2(ownerPos);
        }
    }

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);

        if (fsmStateArg is SkillStateArg skillStateArg)
        {
            m_CanChangeDir = skillStateArg.canChangeDir;
            m_CanMove = skillStateArg.canMove;
            m_Dir = skillStateArg.dir;
        }
        else if (fsmStateArg is MoveStateArg  moveStateArg)
        {
            m_Dir = moveStateArg.dir.x;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_CanMove = false;
    }
}