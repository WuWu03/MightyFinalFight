using GameFrameWork.Fsm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelDrop : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as Barrel;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.gravityScale = 1;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Owner.SetPos(m_Owner.Pos);
        m_Owner.BarrelAnimator.animation.timeScale = 1;
        m_Owner.BarrelAnimator.animation.Play(AnimName.Drop, 1);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if(m_Owner.BarrelAnimator.animation.isCompleted)
        {
            if (m_Owner.BarrelData.MoveSpeed > 0) ChangeState<BarrelMove>(fsm);
            else ChangeState<BarrelIdle>(fsm);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private Barrel m_Owner = null;
}
