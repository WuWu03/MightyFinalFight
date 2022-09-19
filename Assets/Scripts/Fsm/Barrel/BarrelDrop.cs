using GameFrameWork.Fsm;
using System;
using UnityEngine;

public class BarrelDrop : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.onGroundEvent.AddListener(OnGround);
        m_Owner.AddForce(0, 50);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private void OnGround()
    {
        if (m_Owner.barrelData.moveSpeed > 0) ChangeState<BarrelMove>(m_Owner.barrelFsm);
        else ChangeState<BarrelIdle>(m_Owner.barrelFsm);
    }

    private Barrel m_Owner = null;
}
