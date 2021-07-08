using DragonBones;
using GameFrameWork;
using GameFrameWork.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJumpAttackDeployer : SkillBaseDeployer
{
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {

    }

    public override void DeploySkill()
    {
        if (!m_IsOnGround) return;

        m_IsOnGround = false;
        m_CanEffect = true;

        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);

        AttackData attackData = AttackData.Create();
        attackData.SkillID = m_SkillData.ID;
        attackData.AnimName = m_SkillData.AnimationName;
        attackData.AnimSpeed = m_SkillData.AnimSpeed;
        attackData.AnimTime = m_SkillData.AnimTime;
        attackData.Dir = m_Owner.Dir;
        attackData.CanChangeDir = false;
       
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Just)
        {
            attackData.AddSelfForce = m_SkillData.SkillEffects[0].AddSelfForce;
        }

        m_Owner.OnGroundEvent.AddListener(OnGroundEvent);
        m_Owner.OnDropEvent.AddListener(OnDropEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.SetCatch(false);
        m_Owner.OnAttackMsg(attackData, true);

        ReferencePool.Release(attackData);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = m_IsOnGround;//(base.IsAllComplete() && m_Owner.HitSuccess) || m_IsOnGround;

        if (isComplete)
        {
            m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        }

        return isComplete;
    }

    public override void Update()
    {
        base.Update();
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Just)
        {
            if (m_CanEffect)
            {
                base.DeploySkill();
                if (m_Owner.HitSuccess)
                    m_CanEffect = false;
            }
        }
    }

    private void SkillEvent(string type, EventObject eventObject)
    {
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
        {
            base.DeploySkill();
        }
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", eventObject.name);
    }

    private void OnDropEvent()
    {
        m_CanEffect = true;
        m_Owner.OnDropEvent.RemoveListener(OnDropEvent);
    }

    private void OnGroundEvent()
    {
        m_Owner.OnGroundEvent.RemoveListener(OnGroundEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_IsOnGround = true;
        m_CanEffect = false;
    }

    public override void OnExit()
    {
        base.OnExit();
        m_CanEffect = true;
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
    }

    private bool m_CanEffect = true;
    private bool m_IsOnGround = true;
}