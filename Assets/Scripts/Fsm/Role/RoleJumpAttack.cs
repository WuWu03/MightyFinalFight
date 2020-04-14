using FrameWork.Fsm;

public class RoleJumpAttack : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}