using GameFrameWork.Fsm;

public class BarrelDead : FsmState
{
    private float m_AttackerDir;
    private Barrel m_Owner;
    
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
        if (m_Owner.IsCurrAnimationComplete())
        {
            m_Owner.Release();
        }
    }

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);
        
        if (fsmStateArg is HurtStateArg hurtStateArg)
        {
            m_AttackerDir = hurtStateArg.attackerDir;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_AttackerDir = 0f;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }
}
