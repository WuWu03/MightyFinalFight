using GameFrameWork.Fsm;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleSwoon : FsmState
{
    private Vector2 m_AddForce = Vector2.zero;
    private BaseRole m_Owner;
    
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetStateParam(FsmStateMap.GetParam<RoleStateParam>(this.GetType()));
        m_Owner.ResetRigidbody();
        m_Owner.AddForce(m_AddForce);
        m_Owner.onGroundEvent += OnBounce;
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

    protected override void OnSetStateData(Fsm fsm, FsmStateArg fsmStateArg)
    {
        base.OnSetStateData(fsm, fsmStateArg);

        if (fsmStateArg is HurtStateArg hurtStateArg)
        {
            m_AddForce = hurtStateArg.attackForce;
            
            if (fsm.currState != this)
            {
                return;
            }

            if (hurtStateArg.isChangeVelocity)
            {
                m_Owner.SetVelocity(hurtStateArg.changeVelocity);
            }

            m_Owner.AddForce(m_AddForce, true);
            m_Owner.onGroundEvent += OnBounce;
        }
    }

    private void OnBounce()
    {
        m_Owner.SetGravityScale(0.8f);
        m_Owner.SetVelocityY(1.5f, true);
        m_Owner.StopAnimation(AnimName.SwoonUp);
        m_Owner.PlayAnimation(AnimName.SwoonDown);
        m_Owner.onGroundEvent -= OnBounce;
        m_Owner.onDropEvent += OnDrop;
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
    }

    private void OnDrop()
    {
        m_Owner.onDropEvent -= OnDrop;
        m_Owner.onGroundEvent += OnGround;
    }

    private void OnGround()
    {
        m_Owner.onGroundEvent -= OnGround;
        m_Owner.SetPos2(m_Owner.pos);
        m_Owner.SetIsBeThrow(false);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDown));
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
}