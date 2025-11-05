using GameFrameWork.Fsm;
using UnityEngine;

public class RoleMove : FsmState
{
    private bool m_CanChangeDir;
    private BaseRole m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        
        if (m_Owner is BaseHero { weapon: not null })
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move_Weapon, -1, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
        else
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move, -1, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
    }

    protected override void OnFixedUpdate(Fsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_Owner.isPause)
        {
            return;
        }

        if (m_CanChangeDir && !m_Owner.isCatching)
        {
            m_Owner.SetDir(m_Owner.moveDir.x);
        }

        Vector3 ownerPos = m_Owner.transform.localPosition + fixedDeltaTime * m_Owner.entityAttribute.moveSpeed * new Vector3(m_Owner.moveDir.x, m_Owner.moveDir.y, 0);
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);

        if (fsmStateArg is MoveStateArg moveStateArg)
        {
            m_CanChangeDir = moveStateArg.canChangeDir;
        }
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move_Catch);
        m_Owner.StopAnimation(AnimName.Move);
        m_Owner.StopAnimation(AnimName.Move_Weapon);
        m_CanChangeDir = false;
    }

    protected override void OnRelease(Fsm fsm)
    {
        base.OnRelease(fsm);
        m_Owner = null;
    }
}