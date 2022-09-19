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

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
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

    }

    public void SetCanChangeDir(bool value)
    {
        m_CanChangeDir = value;
    }

    private bool m_CanChangeDir;
    private float m_Dir;
    private BaseRole m_Owner = null;
}