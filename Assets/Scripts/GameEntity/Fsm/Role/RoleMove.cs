using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleMove : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        if (m_Owner.objectType == ObjectType.Player && (m_Owner as BaseHero).weapon != null)
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move_Weapon, 0, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
        else
        {
            m_Owner.PlayAnimation(m_Owner.isCatching ? AnimName.Move_Catch : AnimName.Move, 0, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
    }

    protected override void OnFixedUpdate(BaseFsm fsm, float fixedDeltaTime, float fixedUnscaledDeltaTime)
    {
        if (m_CanChangeDir && !m_Owner.isCatching)
        {
            m_Owner.SetDir(m_Owner.moveDir.x);
        }

        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.moveDir.x, m_Owner.moveDir.y) * m_Owner.entityAttribute.moveSpeed * fixedDeltaTime;
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
        MoveStateData moveData = stateData as MoveStateData;
        m_CanChangeDir = moveData.canChangeDir;
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move_Catch);
        m_Owner.StopAnimation(AnimName.Move);
        m_Owner.StopAnimation(AnimName.Move_Weapon);
        m_CanChangeDir = false;
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        base.OnDestroy(fsm);
        m_Owner = null;
    }

    private bool m_CanChangeDir = false;
    private BaseRole m_Owner = null;
}