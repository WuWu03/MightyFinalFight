using GameFrameWork.Fsm;

public class RoleHurt : BaseFsmState
{
    public HurtData HurtData
    {
        get;
        set;
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(HurtData.HurtAnim, 1, m_Owner.IsBeCatch ? 1f : 1.5f);
        m_Owner.SetPos(m_Owner.Pos);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            if (m_Owner.Health <= 0)
            {
                if (m_Owner.IsInGround)
                    ChangeState<RoleDead>(fsm);
            }
            else ChangeState<RoleIdle>(fsm);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Hurt);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}