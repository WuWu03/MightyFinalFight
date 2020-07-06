using FrameWork.Sound;
using UnityEngine;
using static SkillData;

public class SkillSkillAttackDeployer : SkillDeployer
{
    public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) { }

    public override void DeploySkill()
    {
        if (!CheckStatus(m_SkillData.Status))
        {
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            return;
        }

        if (m_SkillData.DeployeType == SkillDeployeType.Just)
        {
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(m_SkillData);
            base.DeploySkill();
            return;
        }

        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnSkillMsg(m_SkillData);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete();

        if(m_SkillData.DeployeType == SkillDeployeType.Animtion)
        {
            isComplete = m_Owner.IsPlayComplete();
        }

        if (isComplete)
        {
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }

        return isComplete;
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

    private bool CheckStatus(SkillStatus status)
    {
        bool ret = false;
        switch (status)
        {
            case SkillStatus.None:
                ret = true;
                break;
            case SkillStatus.Float:
                ret = m_Owner.IsFloat;
                break;
            case SkillStatus.Ground:
                ret = m_Owner.IsInGround;
                break;
            case SkillStatus.Catch:
                ret = (m_Owner as BaseHero).IsCatch;
                break;
        }

        return ret;
    }
}