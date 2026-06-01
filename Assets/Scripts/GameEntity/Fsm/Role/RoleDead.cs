using WuWuFramework.Fsm;
using UnityEngine;

public class RoleDead : FsmState
{
    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseRole m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Dead, 4);
        m_Owner.SetPos2(m_Owner.pos);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsCurrAnimationComplete())
        {
            if (m_Owner.objectType == ObjectType.Player)
            {
                PlayerMgr.instance.Rebirth(m_ReBirthPos);
                return;
            }

            m_Owner.Release();
        }
    }

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);

        if (fsmStateArg is DropTrapStateArg dropTrapStateArg)
        {
            m_ReBirthPos = dropTrapStateArg.rebirthPos;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_ReBirthPos = Vector2.zero;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }
}