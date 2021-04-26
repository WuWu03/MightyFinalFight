using DragonBones;
using GameFrameWork.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJumpAttackDeployer : SkillBaseDeployer
{
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_AttackMsgData = new AttackData();
    }

    public override void DeploySkill()
    {
        if (!m_IsOnGround) return;
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);

        m_AttackMsgData.Dir = m_Owner.Dir;
        m_AttackMsgData.CanChangeDir = false;
        m_AttackMsgData.AnimationName = m_SkillData.AnimationName;
        m_AttackMsgData.AnimSpeed = m_SkillData.AnimSpeed;
        m_AttackMsgData.AnimTime = m_SkillData.AnimTime;
        m_IsOnGround = false;
        m_CanEffect = true;
        m_AttackMsgData.skillID = m_SkillData.ID;

        if (m_SkillData.TriggerType == SkillData.SkillTriggerType.Just)
        {
            m_AttackMsgData.AddSelfForce = m_SkillData.SkillEffects[0].AddSelfForce;
        }

        m_Owner.OnGroundEvent.AddListener(OnGroundEvent);
        m_Owner.OnDropEvent.AddListener(OnDropEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.SetCatch(false);
        m_Owner.OnAttackMsg(m_AttackMsgData, true);
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
        if (m_SkillData.TriggerType == SkillData.SkillTriggerType.Just)
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
        if (m_SkillData.TriggerType == SkillData.SkillTriggerType.Animtion)
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
    private AttackData m_AttackMsgData = null;
}