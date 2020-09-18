using FrameWork.Fsm;
using UnityEngine;

public class RoleAttack : BaseFsmState, IStateParam<AttackData>
{
    public AttackData StateParam
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
        if (StateParam.CanChangeDir)
        {
            m_Owner.SetDir(StateParam.Dir);
        }
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        StateParam = null;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private BaseRole m_Owner = null;
}