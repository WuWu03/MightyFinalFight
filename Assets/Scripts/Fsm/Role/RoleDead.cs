using DG.Tweening.Plugins.Options;
using GameFrameWork.Fsm;
using UnityEngine;
public class RoleDead : BaseFsmState
{
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.PlayAnimation(AnimName.Dead, 4, 1);
        m_Owner.SetPos(m_Owner.Pos);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            if (m_Owner.ObjectType == ObjectType.Player)
            {
                PlayerMgr.Ins.Rebirth(m_ReBirthPos);              
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

    public override void SetParam(object[] args)
    {
        m_ReBirthPos = (Vector2)args[0];
    }

    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseRole m_Owner = null;
}