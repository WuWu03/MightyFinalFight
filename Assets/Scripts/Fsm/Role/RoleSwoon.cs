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
        m_Owner.OnGroundEvent.AddListener(OnGround);
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.Rigidbody.AddForce(Force);
        m_Owner.PlayAnimation(AnimName.SmoonUp, 0);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);

        if (m_IsGround)
        {
            m_Owner.SetPos(m_Owner.Pos);
            if(m_Owner.IsPlayComplete())
            {
                m_IsGround = false;
                GameFrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnFallDown");
            }
        }
    }

    private void OnGround()
    {
        m_IsGround = true;
        m_Owner.Rigidbody.velocity = new Vector2(m_Owner.Rigidbody.velocity.x, -1f);
        m_Owner.StopAnimation(AnimName.SmoonUp);
        m_Owner.PlayAnimation(AnimName.SmoonDown, 1,0.9f);
        GameFrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnFallDown");
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.SmoonDown);
        if (m_IsGround)
            GameFrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnFallDown");
        m_IsGround = false;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private bool m_IsGround = false;
    private BaseRole m_Owner = null;
}