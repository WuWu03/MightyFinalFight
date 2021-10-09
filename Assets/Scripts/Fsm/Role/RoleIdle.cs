using GameFrameWork.Fsm;
using UnityEngine;

public class RoleIdle : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.SetPos(m_Owner.Pos, true);

        if(m_Owner.ObjectType == ObjectType.Player && (m_Owner as BaseHero).Weapon != null)
        {
            m_Owner.PlayAnimation(AnimName.Idle_Weapon, 0);
        }
        else
        {
            m_Owner.PlayAnimation(AnimName.Idle, 0);
        }
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {

    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
        m_Owner.StopAnimation(AnimName.Idle_Weapon);
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}