using GameFrameWork.Fsm;
using System;
using UnityEngine;

public class RoleSwoon : BaseFsmState
{
    public Vector2 Force
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
        m_Owner.OnGroundEvent.AddListener(OnBounce);
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.Rigidbody.AddForce(Force);
        m_Owner.PlayAnimation(AnimName.SwoonUp, 0);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);

        if(m_IsBounce && !m_IsAddGroundEvent)
        {
            m_IsAddGroundEvent = true;
            m_Owner.OnGroundEvent.AddListener(OnGround);
        }
    }

    private void OnBounce()
    {
        m_IsBounce = true;
        m_Owner.Rigidbody.velocity = new Vector2(m_Owner.Rigidbody.velocity.x, 1.5f);
        m_Owner.StopAnimation(AnimName.SwoonUp);
        m_Owner.PlayAnimation(AnimName.SwoonDown,0);
        GameFrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnFallDown");
    }

    private void OnGround()
    {
        m_Owner.SetPos(m_Owner.Pos);
        m_Owner.SetThrow(false);
        GameFrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnFallDown");
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_IsBounce = false;
        m_IsAddGroundEvent = false;
        m_Owner.StopAnimation(AnimName.SwoonDown);
    }

    public override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private bool m_IsAddGroundEvent = false;
    private bool m_IsBounce = false;
    private BaseRole m_Owner = null;
}