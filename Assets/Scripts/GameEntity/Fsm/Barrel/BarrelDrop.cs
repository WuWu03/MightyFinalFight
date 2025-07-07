using GameFrameWork.FSM;
using System;
using UnityEngine;

public class BarrelDrop : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.AddForce(0, 50);
    }

    protected override void OnUpdate(FiniteStateMachine fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.isInGround)
        {
            if (m_Owner.barrelData.moveSpeed > 0)
            {
                ChangeState<BarrelMove>(fsm);
            }
            else
            {
                ChangeState<BarrelIdle>(fsm);
            }
        }
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {

    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private Barrel m_Owner = null;
}
