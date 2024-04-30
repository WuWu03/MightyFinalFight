using DragonBones;
using GameFrameWork;
using GameFrameWork.Audio;
using UnityEngine;
using static SkillConfigData;

public class SkillSkillAttackDeployer : SkillBaseDeployer
{
    public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) { }

    public override void DeploySkill()
    {
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        SkillStateData skillData = SkillStateData.Create();
        skillData.skillID = m_SkillData.Id;
        skillData.animName = m_SkillData.AnimationName;
        skillData.animTime = m_SkillData.AnimTime;
        skillData.animSpeed = m_SkillData.AnimSpeed;
        skillData.canChangeDir = m_SkillData.CanChangeDir;
        skillData.canMove = m_SkillData.CanMove;

        if (m_SkillData.TriggerType != SkillTriggerType.Animtion)
        {
            m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(skillData);
            base.DeploySkill();
            ReferencePool.ReleaseReference(skillData);
            return;
        }

        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnSkillMsg(skillData);
        ReferencePool.ReleaseReference(skillData);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete();

        if (isComplete)
        {
            m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);

            if (!m_SkillData.IsInEffectPlaySound)
            {
                m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
            }
        }

        return isComplete;
    }

    private void SkillEvent(string type, EventObject eventObject)
    {
        base.DeploySkill();
    }

    protected override void OnAnimationEffectComplete()
    {
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, "Sound/" + eventObject.name);
    }

    public override void Exit()
    {
        base.Exit();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        if (!m_SkillData.IsInEffectPlaySound)
        {
            m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        }
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }
}