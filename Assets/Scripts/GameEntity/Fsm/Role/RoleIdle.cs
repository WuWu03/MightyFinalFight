using WuWuFramework.Fsm;

public class RoleIdle : FsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        m_Owner.ResetRigidbody();
        m_Owner.SetPos2(m_Owner.pos, true);

        if (m_Owner.objectType == ObjectType.Player && (m_Owner as BaseHero).weapon != null)
        {
            m_Owner.PlayAnimation(AnimName.Idle_Weapon);
        }
        else
        {
            m_Owner.PlayAnimation(AnimName.Idle);
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Idle);
        m_Owner.StopAnimation(AnimName.Idle_Weapon);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private BaseRole m_Owner = null;
}