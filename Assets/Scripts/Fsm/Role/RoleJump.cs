using GameFrameWork.Fsm;
using UnityEngine;

public class RoleJump : BaseFsmState
{
    public float Dir
    {
        set
        {
            m_Dir = value;
        }
    }


    public bool CanChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.AddForce(new Vector2(m_Dir * m_Owner.JumpForce.x, m_Owner.JumpForce.y));
        m_Owner.PlayAnimation(AnimName.JumpUp);
        m_HasAddXForce = m_Dir != 0;
        m_Owner.SetDir(m_Dir);
        m_Owner.OnDropEvent.AddListener(OnDrop);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsFloat)
        {
            if (Mathf.Abs(m_Dir) > 0.01f && !m_HasAddXForce)
            {
                m_HasAddXForce = true;
                m_Owner.Rigidbody.AddForce(Vector2.right * m_Dir * m_Owner.JumpForce.x, 0f);
                if (m_CanChangeDir)
                    m_Owner.SetDir(m_Dir);
            }

            if (m_HasAddXForce)
            {
                m_Owner.PlayAnimation(AnimName.JumpRoll, -1, 0.5f);
            }
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_HasAddXForce = false;
        m_Owner.StopAnimation(AnimName.JumpUp);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private void OnDrop()
    {
        if (!m_Owner.IsAnyState(typeof(RoleAttack)))
            m_Owner.PlayAnimation(AnimName.JumpDown);
    }

    private float m_Dir = 0;
    private bool m_CanChangeDir = false;
    private bool m_HasAddXForce = false;
    private BaseRole m_Owner = null;
}