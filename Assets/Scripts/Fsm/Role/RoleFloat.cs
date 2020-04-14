using FrameWork.Fsm;
using UnityEngine;

public class RoleFloat : BaseFsmState
{
    public Vector2 Force
    {
        get;
        set;
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.AddForce(new Vector2(Force.x * m_Owner.Dir, Force.y));
        m_Owner.PlayAnimation(AnimName.SmoonDown);
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (!m_Owner.IsInGround)
        {
            return;
        }

        if (!m_Owner.Rigidbody.bodyType.Equals(RigidbodyType2D.Kinematic))
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            m_Owner.Rigidbody.velocity = new Vector2(m_Owner.Rigidbody.velocity.x, -0.001f);
            m_Owner.StopAnimation(AnimName.SmoonUp);
            m_Owner.PlayAnimation(AnimName.SmoonDown, 1);
            m_Owner.SetPos(m_Owner.Pos);
        }

        if (!m_Owner.ActorAnimator.animation.isCompleted)
        {
            return;
        }

        m_Owner.Rigidbody.velocity = Vector2.zero;

        if (m_Owner.Health > 0)
        {
            ChangeState<RoleAwaken>(fsm);
        }
        else if (m_Owner.IsInGround)
        {
            ChangeState<RoleDead>(fsm);
        }
    }

    private BaseRole m_Owner = null;
}