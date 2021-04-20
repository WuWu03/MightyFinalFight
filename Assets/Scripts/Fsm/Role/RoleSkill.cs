using GameFrameWork.Fsm;

public class RoleSkill : BaseFsmState
{
    public bool CanChangeDir = false;
    public float Dir = 1f;
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        Dir = m_Owner.Dir;
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (CanChangeDir)
        {
            m_Owner.SetDir(Dir);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        CanChangeDir = false;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}