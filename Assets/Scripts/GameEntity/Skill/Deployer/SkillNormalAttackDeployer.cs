using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.Utils;
using System.Collections.Generic;

public class SkillNormalAttackDeployer : SkillBaseDeployer
{
    public SkillNormalAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_QueueSound = new Queue<string>();
    }

    public override void DeploySkill()
    {
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        SkillStateData skillStateData = SkillStateData.Create();
        skillStateData.dir = m_Owner.dir;
        skillStateData.skillID = mSkillData.id;
        skillStateData.animName = mSkillData.AnimationName;
        skillStateData.animSpeed = mSkillData.AnimSpeed;
        skillStateData.animTime = mSkillData.AnimTime;
        skillStateData.canChangeDir = mSkillData.CanChangeDir;

        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnAttackMsg(skillStateData);
        skillStateData.Release();
    }

    private void SkillEvent(string type, EventObject eventObject)
    {
        base.DeploySkill();
        RealPlaySound();
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        m_QueueSound.Enqueue(eventObject.name);
    }

    protected override void OnAnimationEffectComplete()
    {
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private void RealPlaySound()
    {
        if (m_QueueSound.Count < 1)
        {
            return;
        }

        string soundName = m_QueueSound.Dequeue();

        if (!m_Owner.isHitSuccess || mSkillData.IsInEffectPlaySound)
        {
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound", soundName, ".ogg"));
        }
    }

    public override void Exit()
    {
        base.Exit();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private Queue<string> m_QueueSound = null;
}