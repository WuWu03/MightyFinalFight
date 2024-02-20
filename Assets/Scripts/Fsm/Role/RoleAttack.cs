using GameFrameWork.Fsm;
using UnityEngine;

public class RoleAttack : BaseFsmState
{
    public float dir
    {
        set
        {
            m_Dir = value;
        }
    }


    public bool canChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_CanChangeDir = false;
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private bool m_CanChangeDir;
    private float m_Dir;
    private BaseRole m_Owner = null;
}