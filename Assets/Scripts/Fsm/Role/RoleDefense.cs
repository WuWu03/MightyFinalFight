using FrameWork.Fsm;
using UnityEngine;

public class RoleDefense : BaseFsmState
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
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}