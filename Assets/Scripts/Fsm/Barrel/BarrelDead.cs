using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelDead : BaseFsmState
{
    public float attackerDir
    {
        set
        {
            m_AttackerDir = value;
        }
    }

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

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private float m_AttackerDir = 0f;
    private Barrel m_Owner = null;
}
