using DG.Tweening.Plugins.Options;
using GameFrameWork.Fsm;
using UnityEngine;
public class RoleDead : BaseFsmState
{
    public Vector2 rebirthPos
    {
        set
        {
            m_ReBirthPos = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.PlayAnimation(AnimName.Dead, 4, 1);
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetThrow(false);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
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

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_ReBirthPos = Vector2.zero;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseRole m_Owner = null;
}