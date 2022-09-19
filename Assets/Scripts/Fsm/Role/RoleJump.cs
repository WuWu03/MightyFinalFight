using GameFrameWork.Fsm;
using UnityEngine;

public class RoleJump : BaseFsmState
{
    public float dir
    {
        set
        {
            m_Dir = value;
        }
    }


    public bool canChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    public bool isCatch
    {
        set
        {
            m_IsCatch = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.AddForce(m_Dir * m_Owner.entityAttribute.jumpForce.x, m_Owner.entityAttribute.jumpForce.y);
        m_Owner.PlayAnimation(m_IsCatch ? AnimName.Catch : AnimName.JumpUp);
        m_HasAddXForce = m_Dir != 0;

        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }

        m_Owner.onDropEvent.AddListener(OnDrop);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
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

            if (m_HasAddXForce && !m_IsCatch)
            {
                m_Owner.PlayAnimation(AnimName.JumpRoll, -1, 0.5f);
            }
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Dir = 0;
        m_CanChangeDir = false;
        m_HasAddXForce = false;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private void OnDrop()
    {
        if (!m_IsCatch && !m_Owner.IsAnyState(typeof(RoleAttack)))
        {
            m_Owner.PlayAnimation(AnimName.JumpDown);
        }
    }

    private float m_Dir = 0;
    private bool m_CanChangeDir = false;
    private bool m_HasAddXForce = false;
    private bool m_IsCatch = false;
    private BaseRole m_Owner = null;
}