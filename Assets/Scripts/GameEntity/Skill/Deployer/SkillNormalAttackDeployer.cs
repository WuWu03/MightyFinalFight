using DragonBones;
using GameFrameWork;
using GameFrameWork.Sound;
using System.Collections.Generic;
using UnityEngine;

public class SkillNormalAttackDeployer : SkillBaseDeployer
{
    public SkillNormalAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_QueueSound = new Queue<string>();
    }

    public override void DeploySkill()
    {
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);

        AttackData attackData = AttackData.Create();
        attackData.Dir = m_Owner.Dir;
        attackData.SkillID = m_SkillData.ID;
        attackData.AnimName = m_SkillData.AnimationName;
        attackData.AnimSpeed = m_SkillData.AnimSpeed;
        attackData.AnimTime = m_SkillData.AnimTime;
        attackData.CanChangeDir = m_SkillData.CanChangeDir;

        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Just)
        {
            attackData.AddSelfForce = m_SkillData.SkillEffects[0].AddSelfForce;
        }

        m_Owner.ActorAnimator.AddEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnAttackMsg(attackData);

        ReferencePool.Release(attackData);
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

    protected override void OnEffectComplete()
    {
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
    }

    private void RealPlaySound()
    {
        if (m_QueueSound.Count < 1) return;
        string soundName = m_QueueSound.Dequeue();

        if (m_Owner.HitSuccess)
        {
            if (m_SkillData.IsInEffectPlaySound) SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/" + soundName);
        }
        else
        {
            SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/" + soundName);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(EventObject.SOUND_EVENT, SoundEvent);
    }

    private Queue<string> m_QueueSound = null;
}