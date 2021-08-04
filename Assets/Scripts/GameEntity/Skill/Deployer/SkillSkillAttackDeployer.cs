using DragonBones;
using GameFrameWork.Sound;
using UnityEngine;
using static SkillConfigData;

public class SkillSkillAttackDeployer : SkillBaseDeployer
{
    public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) { }

    public override void DeploySkill()
    {
        m_EnternalTriggerTimer = Time.time;
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);

        if (m_SkillData.TriggerType == SkillTriggerType.Just)
        {
            m_Owner.OnSkillMsg(m_SkillData);
            base.DeploySkill();
            return;
        }

        m_Owner.ActorAnimator.AddEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnSkillMsg(m_SkillData);
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete();

        if(m_SkillData.TriggerType == SkillTriggerType.Enternal)
        {
            isComplete = isComplete && Time.time - m_EnternalTriggerTimer >= m_SkillData.EnternalTiggerTime;
        }

        if (isComplete)
        {
            m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);

            if (!m_SkillData.IsInEffectPlaySound)
                m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
        }

        return isComplete;
    }

    public override void Update()
    {
        if(m_SkillData.TriggerType == SkillTriggerType.Enternal)
        {
            base.DeploySkill();
        }

        base.Update();
    }

    private void SkillEvent(string type, EventObject eventObject)
    {
        if (CurrEffect.AddSelfForce != Vector2.zero)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Owner.Rigidbody.AddForce(new Vector2(CurrEffect.AddSelfForce.x * m_Owner.Dir, CurrEffect.AddSelfForce.y));
        }
        base.DeploySkill();
    }

    protected override void OnEffectComplete()
    {
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/" + eventObject.name);
    }

    public override void OnExit()
    {
        base.OnExit();
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        if (!m_SkillData.IsInEffectPlaySound)
            m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
    }

    private float m_EnternalTriggerTimer = 0f;
}