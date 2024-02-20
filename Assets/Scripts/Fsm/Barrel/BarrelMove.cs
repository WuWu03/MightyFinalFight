using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelMove : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.Move, 0, m_Owner.barrelData.moveSpeed * 0.5f);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        m_Owner.SetDir(m_Owner.barrelData.dir);
        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.barrelData.dir, 0, 0) * m_Owner.barrelData.moveSpeed * Time.deltaTime;
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move);
    }

    private Barrel m_Owner = null;
}
