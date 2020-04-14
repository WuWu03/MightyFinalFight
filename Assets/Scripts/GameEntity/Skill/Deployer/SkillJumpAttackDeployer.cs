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
        m_AttackMsgData.Dir = m_Owner.Dir;
        m_AttackMsgData.CanChangeDir = false;
        m_AttackMsgData.AnimationName = m_SkillData.AnimationName;
        m_Owner.ActorAnimator.AddDBEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnAttackMsg(m_AttackMsgData);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete() && m_Owner.GetComponent<AvatarCtrl>().AttackSuccess;
        isComplete = isComplete || m_Owner.IsInGround;

        if (isComplete)
        {
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
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

    private AttackData m_AttackMsgData = null;
}