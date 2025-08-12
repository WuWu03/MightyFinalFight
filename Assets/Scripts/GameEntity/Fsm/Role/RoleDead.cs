using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleDead : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Dead, 4);
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetThrow(false);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            if (m_Owner.objectType == ObjectType.Player)
            {
                PlayerMgr.instance.Rebirth(m_ReBirthPos);
                return;
            }

            m_Owner.Release();
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
        DropTrapStateData trapData = stateData as DropTrapStateData;
        m_ReBirthPos = trapData.rebirthPos;
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_ReBirthPos = Vector2.zero;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseRole m_Owner = null;
}