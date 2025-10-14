using GameFrameWork;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleMove : BaseFsmState
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
        if (m_Owner.objectType == ObjectType.Player && (m_Owner as BaseHero).weapon != null)
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move_Weapon, -1, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
        else
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move, -1, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
    }

    protected override void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_Owner.isPause)
        {
            return;
        }

        if (m_CanChangeDir && !m_Owner.isCatching)
        {
            m_Owner.SetDir(m_Owner.moveDir.x);
        }

        Vector3 ownerPos = m_Owner.transform.localPosition + fixedDeltaTime * m_Owner.entityAttribute.moveSpeed * new Vector3(m_Owner.moveDir.x, m_Owner.moveDir.y, 0);
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnSetStateData(Fsm fsm, GameFrameWorkEventArg stateData)
    {
        base.OnSetStateData(fsm, stateData);
        MoveStateData moveData = stateData as MoveStateData;
        m_CanChangeDir = moveData.canChangeDir;
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move_Catch);
        m_Owner.StopAnimation(AnimName.Move);
        m_Owner.StopAnimation(AnimName.Move_Weapon);
        m_CanChangeDir = false;
    }

    protected override void OnRelease(Fsm fsm)
    {
        base.OnRelease(fsm);
        m_Owner = null;
    }

    private bool m_CanChangeDir = false;
    private BaseRole m_Owner = null;
}