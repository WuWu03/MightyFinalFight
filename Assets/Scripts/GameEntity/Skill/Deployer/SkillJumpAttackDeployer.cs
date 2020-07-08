using FrameWork.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJumpAttackDeployer : SkillDeployer
{
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_AttackMsgData = new AttackData();
    }

    public override void DeploySkill()
    {
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);

        m_AttackMsgData.Dir = m_Owner.Dir;
        m_AttackMsgData.CanChangeDir = false;
        m_AttackMsgData.AnimationName = m_SkillData.AnimationName;
        m_AttackMsgData.AnimSpeed = m_SkillData.AnimSpeed;
        m_AttackMsgData.AnimTime = m_SkillData.AnimTime;
        m_IsComplete = false;

        if (m_SkillData.DeployeType == SkillData.SkillDeployeType.Just)
        {
            m_AttackMsgData.AddSelfForce = m_SkillData.SkillEffects[0].AddSelfForce;
        }

        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnGroundEvent.AddListener(OnGroundEvent);
        m_Owner.OnAttackMsg(m_AttackMsgData, true);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete() && m_IsComplete;
;
        if (isComplete)
        {
            m_IsComplete = false;
            m_Owner.OnGroundEvent.RemoveListener(OnGroundEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }

        return isComplete;
    }

    public override void Update()
    {
        base.DeploySkill();
    }

    private void SkillEvent(string type, DragonBones.EventObject eventObject)
    {
        base.DeploySkill();
    }

    private void SoundEvent(string type, DragonBones.EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", eventObject.name);
    }

    private void OnGroundEvent()
    {
        m_IsComplete = true;
    }

    private bool m_IsComplete = false;
    private AttackData m_AttackMsgData = null;
}