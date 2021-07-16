using GameFrameWork.Fsm;
using UnityEngine;

public class RoleAttack : BaseFsmState
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
        if (m_CanChangeDir)
        {
            m_Owner.SetDir(m_Dir);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {

    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    public override void SetParam(object[] args)
    {
        m_CanChangeDir = (bool)args[0];
        m_Dir = (float)args[1];
    }

    private bool m_CanChangeDir;
    private float m_Dir;
    private BaseRole m_Owner = null;
}