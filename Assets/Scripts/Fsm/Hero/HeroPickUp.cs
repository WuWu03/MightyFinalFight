using GameFrameWork.Fsm;
using UnityEngine;

public class HeroPickUp : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.gravityScale = 1;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Owner.PlayAnimation(AnimName.PickUp, 1, 1f);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            fsm.SetDefaultState<RoleIdle>();
            fsm.ChangeDefaultState();
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.PickUp);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}
