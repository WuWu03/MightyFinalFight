using WuWuFramework.Fsm;
using UnityEngine;

public class BarrelMove : FsmState
{
    private Barrel m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.Move, 0, m_Owner.barrelData.moveSpeed * 0.5f);
    }

    protected override void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        m_Owner.SetDir(m_Owner.barrelData.dir);
        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.barrelData.dir, 0, 0) * m_Owner.barrelData.moveSpeed * fixedDeltaTime;
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move);
    }
}
