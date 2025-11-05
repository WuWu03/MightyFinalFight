using GameFrameWork.Fsm;

public class RoleAwaken : FsmState
{
    private BaseRole m_Owner;
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Awaken, 1, 0.2f);
        m_Owner.SetPos2(m_Owner.pos);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsCurrAnimationComplete())
        {
            fsm.SetDefaultState<RoleIdle>();
            fsm.ChangeDefaultState();
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Awaken);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }
}