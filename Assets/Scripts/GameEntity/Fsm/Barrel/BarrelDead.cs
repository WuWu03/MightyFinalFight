using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelDead : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Dead, 1);
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetDir(-m_AttackerDir);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.armatureAnimator.animation.isCompleted)
        {
            m_Owner.Release();
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
        HurtStateData hurtData = stateData as HurtStateData;
        m_AttackerDir = hurtData.attackerDir;
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_AttackerDir = 0f;
    }

    protected override void OnRelease(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private float m_AttackerDir = 0f;
    private Barrel m_Owner = null;
}
