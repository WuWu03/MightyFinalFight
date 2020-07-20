using FrameWork.Sound;
using UnityEngine;
using static SkillData;

public class SkillSkillAttackDeployer : SkillBaseDeployer
{
    public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) { }

    public override void DeploySkill()
    {
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);

        if (m_SkillData.DeployeType == SkillDeployeType.Just)
        {
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

        if (isComplete)
        {
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);

            if (!m_SkillData.IsInEffectPlaySound)
                m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        }

        return isComplete;
    }

    private void SkillEvent(string type, DragonBones.EventObject eventObject)
    {
        if (CurrEffect.AddSelfForce != Vector2.zero)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Owner.Rigidbody.AddForce(new Vector2(CurrEffect.AddSelfForce.x * m_Owner.Dir, CurrEffect.AddSelfForce.y));
        }
        base.DeploySkill();
    }

    private void SoundEvent(string type, DragonBones.EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", eventObject.name);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

    public override void OnExit()
    {
        base.OnExit();
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        if (!m_SkillData.IsInEffectPlaySound)
            m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

}