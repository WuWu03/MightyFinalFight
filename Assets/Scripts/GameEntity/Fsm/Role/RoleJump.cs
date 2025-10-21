using GameFrameWork.Fsm;
using UnityEngine;

public class RoleJump : FsmState
{
    private float m_Dir;
    private bool m_CanChangeDir;
    private bool m_HasAddXForce;
    private BaseRole m_Owner;

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

        m_Owner.onDropEvent += OnDrop;
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

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);

        if (fsmStateArg is JumpStateArg jumpStateArg)
        {
            m_CanChangeDir = !jumpStateArg.isCatch && jumpStateArg.canChangeDir;
            m_Dir = jumpStateArg.dir.x;
        }
        else if (fsmStateArg is MoveStateArg moveStateArg)
        {
            m_Dir = moveStateArg.dir.x;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Dir = 0;
        m_CanChangeDir = false;
        m_HasAddXForce = false;
        m_Owner.onDropEvent -= OnDrop;
    }

    protected override void OnRelease(Fsm fsm)
    {
        base.OnRelease(fsm);
        m_Owner = null;
    }

    private void OnDrop()
    {
        m_Owner.onDropEvent -= OnDrop;
        if (!m_Owner.isCatching && !m_Owner.IsAnyState(typeof(RoleSkill)))
        {
            m_Owner.PlayAnimation(AnimName.JumpDown);
        }
    }
}