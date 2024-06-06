using DragonBones;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SkillJumpAttackDeployer : SkillBaseDeployer
{
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {

    }

    public override void DeploySkill()
    {
        if (!m_IsOnGround)
        {
            return;
        }

        m_IsOnGround = false;
        m_CanEffect = true;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        AttackStateData attackData = AttackStateData.Create();
        attackData.skillID = m_SkillData.id;
        attackData.animName = m_SkillData.AnimationName;
        attackData.animSpeed = m_SkillData.AnimSpeed;
        attackData.animTime = m_SkillData.AnimTime;
        attackData.dir = m_Owner.dir;
        attackData.canChangeDir = false;
       
        m_Owner.onGroundEvent.AddListener(OnGroundEvent);
        m_Owner.onDropEvent.AddListener(OnDropEvent);
        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.SetCatch(false);
        m_Owner.OnAttackMsg(attackData, true);

        ReferencePool.ReleaseReference(attackData);
    }


    public override bool IsAllComplete()
    {
        if (m_IsOnGround)
        {
            m_CanEffect = false;
            m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        }

        return m_IsOnGround;
    }

    public override void Update()
    {
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Just)
        {
            if (m_CanEffect)
            {
                if (m_Owner.currCtrl.isHitSuccess)
                {
                    m_CanEffect = false;
                    return;
                }

                base.DeploySkill();
            }
        }

        base.Update();
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
        AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, StringUtil.Format("Sound/", eventObject.name, ".ogg"));
    }

    private void OnDropEvent()
    {
        m_Owner.currCtrl.SetHitState(false);
        m_CanEffect = true;
    }

    private void OnGroundEvent()
    {
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_IsOnGround = true;
        m_CanEffect = false;
    }

    public override void Exit()
    {
        base.Exit();
        m_CanEffect = true;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private bool m_CanEffect = false;
    private bool m_IsOnGround = true;
}