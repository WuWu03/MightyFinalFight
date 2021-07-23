using GameFrameWork.Fsm;
using UnityEngine;

public class BarrelDead : BaseFsmState
{
    public float AttackerDir
    {
        set
        {
            m_AttackerDir = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as Barrel;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.BarrelAnimator.animation.timeScale = 1;
        m_Owner.BarrelAnimator.animation.Play(AnimName.Dead, 1);
        m_Owner.SetPos(m_Owner.Pos);
        m_Owner.SetDir(-m_AttackerDir);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.BarrelAnimator.animation.isCompleted)
        {
            m_Owner.Release();
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private float m_AttackerDir = 0f;
    private Barrel m_Owner = null;
}
