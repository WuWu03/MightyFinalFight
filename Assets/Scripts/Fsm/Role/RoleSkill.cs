using GameFrameWork.Fsm;

public class RoleSkill : BaseFsmState
{
    public float Dir
    {
        set
        {
            m_Dir = value;
        }
    }


    public bool CanChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {

    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private bool m_CanChangeDir = false;
    private float m_Dir = 1f;
    private BaseRole m_Owner = null;
}