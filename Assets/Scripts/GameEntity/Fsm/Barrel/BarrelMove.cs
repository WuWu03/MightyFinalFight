using GameFrameWork.FSM;
using UnityEngine;

public class BarrelMove : BaseFsmState
{
    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.PlayAnimation(AnimName.Move, 0, m_Owner.barrelData.moveSpeed * 0.5f);
    }

    protected override void OnFixedUpdate(FiniteStateMachine fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        m_Owner.SetDir(m_Owner.barrelData.dir);
        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.barrelData.dir, 0, 0) * m_Owner.barrelData.moveSpeed * fixedDeltaTime;
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move);
    }

    private Barrel m_Owner = null;
}
