using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelMove : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as Barrel;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.BarrelAnimator.animation.timeScale = m_Owner.BarrelData.MoveSpeed * 0.5f;
        m_Owner.BarrelAnimator.animation.Play(AnimName.Move, 0);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        m_Owner.SetDir(-m_Owner.BarrelData.Dir);
        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.BarrelData.Dir, 0, 0) * m_Owner.BarrelData.MoveSpeed * Time.deltaTime;
        m_Owner.SetPos(ownerPos);
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.BarrelAnimator.animation.Stop(AnimName.Move);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    public override void SetParam(object[] args)
    {

    }

    private Barrel m_Owner = null;
}
