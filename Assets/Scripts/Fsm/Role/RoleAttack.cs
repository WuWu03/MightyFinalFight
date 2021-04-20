using GameFrameWork.Fsm;
using UnityEngine;

public class RoleAttack : BaseFsmState
{
    public AttackData AttackData
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
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (AttackData.CanChangeDir)
        {
            m_Owner.SetDir(AttackData.Dir);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        AttackData = null;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}