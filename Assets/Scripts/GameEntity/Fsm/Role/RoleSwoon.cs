using GameFrameWork;
using GameFrameWork.Fsm;
using System;
using UnityEngine;

public class RoleSwoon : BaseFsmState
{
    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.onGroundEvent.AddListener(OnBounce);
        m_Owner.AddForce(m_Force);
        m_Owner.PlayAnimation(AnimName.SwoonUp);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        m_Owner.UpdatePosX(m_Owner.transform.localPosition.x);

        if(m_IsBounce && !m_IsAddGroundEvent)
        {
            m_IsAddGroundEvent = true;
            m_Owner.onGroundEvent.AddListener(OnGround);
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
        HurtStateData data = stateData as HurtStateData;
        m_Force = data.attackForce;
    }

    private void OnBounce()
    {
        m_IsBounce = true;
        m_Owner.SetGravityScale(0.8f);
        m_Owner.SetVelocityY(1.5f);
        m_Owner.StopAnimation(AnimName.SwoonUp);
        m_Owner.PlayAnimation(AnimName.SwoonDown);
        GameFrameWork.Audio.AudioMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnFallDown");
    }

    private void OnGround()
    {
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetThrow(false);
        GameFrameWork.Audio.AudioMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnFallDown");
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_IsBounce = false;
        m_IsAddGroundEvent = false;
        m_Force = Vector2.zero;
        m_Owner.StopAnimation(AnimName.SwoonDown);
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private Vector2 m_Force = Vector2.zero;
    private bool m_IsAddGroundEvent = false;
    private bool m_IsBounce = false;
    private BaseRole m_Owner = null;
}