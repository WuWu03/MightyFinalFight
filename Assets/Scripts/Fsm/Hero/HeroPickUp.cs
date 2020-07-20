using FrameWork.Fsm;


public class HeroPickUp : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(AnimName.PickUp, 1, 0.4f);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            ChangeState<RoleIdle>(fsm);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.PickUp);
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}
