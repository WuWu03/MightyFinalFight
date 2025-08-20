using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelDead : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as Barrel;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Dead, 1);
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetDir(-m_AttackerDir);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            m_Owner.Release();
        }
    }

    protected override void OnSetStateData(Fsm fsm, BaseEventArgs stateData)
    {
        base.OnSetStateData(fsm, stateData);
        HurtStateData hurtData = stateData as HurtStateData;
        m_AttackerDir = hurtData.attackerDir;
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_AttackerDir = 0f;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private float m_AttackerDir = 0f;
    private Barrel m_Owner = null;
}
