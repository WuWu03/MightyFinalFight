using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Event;
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
        m_Owner.AddForce(m_AddForce);
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
    }

    protected override void OnSetStateData(Fsm fsm, GameFrameWorkEventArg stateData)
    {
        base.OnSetStateData(fsm, stateData);
        HurtStateData data = stateData as HurtStateData;
        m_AddForce = data.attackForce;

        if (fsm.currState != this)
        {
            return;
        }

        if (data.isChangeVelocity)
        {
            m_Owner.SetVelocity(data.changeVelocity);
        }

        m_Owner.AddForce(m_AddForce, true);

        if (data.isSwoon)
        {
            m_Owner.onGroundEvent.RemoveListener(OnBounce);
            m_Owner.onGroundEvent.AddListener(OnBounce);
        }
        else
        {
            m_Owner.onGroundEvent.RemoveListener(OnGround);
            m_Owner.onGroundEvent.AddListener(OnGround);
        }
    }

    private void OnBounce()
    {
        m_Owner.SetGravityScale(0.8f);
        m_Owner.SetVelocityY(1.5f, true);
        m_Owner.StopAnimation(AnimName.SwoonUp);
        m_Owner.PlayAnimation(AnimName.SwoonDown);
        m_Owner.onGroundEvent.RemoveListener(OnBounce);
        m_Owner.onGroundEvent.AddListener(OnGround);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
    }

    private void OnGround()
    {
        m_Owner.onGroundEvent.RemoveListener(OnGround);
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetIsBeThrow(false);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_AddForce = Vector2.zero;
        m_Owner.StopAnimation(AnimName.SwoonDown);
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private Vector2 m_AddForce = Vector2.zero;
    private BaseRole m_Owner = null;
}