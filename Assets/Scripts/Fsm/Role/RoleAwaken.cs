using GameFrameWork.Fsm;
using UnityEngine;

public class RoleAwaken : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.PlayAnimation(AnimName.Awaken, 1, 0.2f);
        m_Owner.SetPos(m_Owner.Pos);    
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            ChangeState<RoleIdle>(fsm);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Awaken);
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}