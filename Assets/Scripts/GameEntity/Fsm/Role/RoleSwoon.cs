using GameFrameWork;
using GameFrameWork.Fsm;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleSwoon : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(false);
        m_Owner.SetCanBeHit(true);
        m_Owner.SetCanJump(false);
        m_Owner.SetCanMove(false);
        m_Owner.SetCanSkill(false);
        m_Owner.SetCanBeCatch(false);
        m_Owner.ResetRigidbody();
        m_Owner.AddForce(m_Force);
        m_Owner.onGroundEvent.AddListener(OnBounce);
        m_Owner.PlayAnimation(AnimName.SwoonUp);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.isPause)
        {
            return;
        }

        m_Owner.UpdatePosX(m_Owner.transform.localPosition.x);

        if (m_IsBounce && !m_IsAddGroundEvent)
        {
            m_IsBounce = false;
            m_IsAddGroundEvent = true;
            m_Owner.onGroundEvent.AddListener(OnGround);
        }
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
        HurtStateData data = stateData as HurtStateData;
        m_Force = data.attackForce;
        m_Owner.AddForce(m_Force);
        m_IsBounce = true;
        m_IsAddGroundEvent = false;

        if (data.isSwoon)
        {
            m_Owner.onGroundEvent.RemoveListener(OnBounce);
            m_Owner.onGroundEvent.AddListener(OnBounce);
            m_IsBounce = false;
            m_IsAddGroundEvent = false;
        }
    }

    private void OnBounce()
    {
        m_IsBounce = true;
        m_IsAddGroundEvent = false;
        m_Owner.SetGravityScale(0.8f);
        m_Owner.SetVelocityY(1.5f);
        m_Owner.StopAnimation(AnimName.SwoonUp);
        m_Owner.PlayAnimation(AnimName.SwoonDown);
        GameFrameWork.Audio.AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
    }

    private void OnGround()
    {
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetIsBeThrow(false);
        GameFrameWork.Audio.AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_IsBounce = false;
        m_IsAddGroundEvent = false;
        m_Force = Vector2.zero;
        m_Owner.StopAnimation(AnimName.SwoonDown);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private Vector2 m_Force = Vector2.zero;
    private bool m_IsAddGroundEvent = false;
    private bool m_IsBounce = false;
    private BaseRole m_Owner = null;
}