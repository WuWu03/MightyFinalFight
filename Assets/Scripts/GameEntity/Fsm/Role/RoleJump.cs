using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleJump : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(true);
        m_Owner.SetCanBeHit(true);
        m_Owner.SetCanJump(false);
        m_Owner.SetCanMove(true);
        m_Owner.SetCanSkill(true);
        m_Owner.SetCanBeCatch(false);
        m_Owner.AddForce(m_Dir * m_Owner.entityAttribute.jumpForce.x, m_Owner.entityAttribute.jumpForce.y);
        m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Catch : AnimName.JumpUp);
        m_HasAddXForce = m_Dir != 0;

        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }

        m_Owner.onDropEvent.AddListener(OnDrop);
    }

    protected override void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_Owner.isFloat)
        {
            if (Mathf.Abs(m_Dir) > 0.01f && !m_HasAddXForce)
            {
                m_HasAddXForce = true;
                m_Owner.AddForce(m_Dir * m_Owner.entityAttribute.jumpForce.x, 0f);

                if (m_CanChangeDir)
                {
                    m_Owner.SetDir(m_Dir);
                }
            }

            if (m_HasAddXForce && !m_Owner.isCatching && m_Owner.objectType == ObjectType.Player)
            {
                m_Owner.PlayAnimation(AnimName.JumpRoll, -1, 0.5f);
            }
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);

        if(stateData is JumpStateData)
        {
            JumpStateData jumpData = stateData as JumpStateData;
            m_CanChangeDir = !jumpData.isCatch && jumpData.canChangeDir;
            m_Dir = jumpData.dir.x;
        }
        else if(stateData is MoveStateData)
        {
            MoveStateData moveData = stateData as MoveStateData;
            m_Dir = moveData.dir.x;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Dir = 0;
        m_CanChangeDir = false;
        m_HasAddXForce = false;
        m_Owner.onDropEvent.RemoveListener(OnDrop);
    }

    protected override void OnRelease(Fsm fsm)
    {
        base.OnRelease(fsm);
        m_Owner = null;
    }

    private void OnDrop()
    {
        if (!m_Owner.isCatching && !m_Owner.IsAnyState(typeof(RoleSkill)))
        {
            m_Owner.PlayAnimation(AnimName.JumpDown);
        }
    }

    private float m_Dir = 0;
    private bool m_CanChangeDir = false;
    private bool m_HasAddXForce = false;
    private BaseRole m_Owner = null;
}