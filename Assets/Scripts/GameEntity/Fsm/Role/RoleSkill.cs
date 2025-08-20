using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleSkill : BaseFsmState
{
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

    protected override void OnSetStateData(Fsm fsm, BaseEventArgs stateData)
    {
        base.OnSetStateData(fsm, stateData);

        if (stateData is SkillStateData)
        {
            SkillStateData skillData = stateData as SkillStateData;
            m_CanChangeDir = skillData.canChangeDir;
            m_CanMove = skillData.canMove;
            m_Dir = skillData.dir;
        }
        else if (stateData is MoveStateData)
        {
            MoveStateData moveData = stateData as MoveStateData;
            m_Dir = moveData.dir.x;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_CanMove = false;
    }

    private bool m_CanMove = false;
    private bool m_CanChangeDir = false;
    private float m_Dir = 0;
    private BaseRole m_Owner = null;
}