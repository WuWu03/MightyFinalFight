using FrameWork.Fsm;
using UnityEngine;

public class RoleJump : BaseFsmState, IStateParam<JumpData>
{
    public JumpData StateParam
    {
        get;
        set;
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.AddForce(new Vector2(StateParam.Dir.x * m_Owner.JumpForce.x, m_Owner.JumpForce.y));
        m_Owner.PlayAnimation(AnimName.JumpUp);
        m_HasAddXForce = StateParam.Dir.x != 0;
        m_Owner.SetDir(StateParam.Dir.x);
        m_Owner.OnGroundEvent.AddListener(OnGround);
        m_Owner.OnDropEvent.AddListener(OnDrop);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsFloat)
        {
            if (Mathf.Abs(StateParam.Dir.x) > 0.01f && !m_HasAddXForce)
            {
                m_HasAddXForce = true;
                m_Owner.Rigidbody.AddForce(Vector2.right * StateParam.Dir.x * m_Owner.JumpForce.x, 0f);
                if (StateParam.CanChangeDir)
                    m_Owner.SetDir(StateParam.Dir.x);
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
        StateParam = null;
        m_Owner.StopAnimation(AnimName.JumpUp);
        Debug.Log("跳跃退出了");
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private void OnGround()
    {

    }

    private void OnDrop()
    {
        if (!m_Owner.IsAnyState(typeof(RoleAttack)))
            m_Owner.PlayAnimation(AnimName.JumpDown);
    }

    private bool m_HasAddXForce = false;
    private BaseRole m_Owner = null;
}