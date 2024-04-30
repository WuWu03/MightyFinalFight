using DragonBones;
using GameFrameWork;
using GameFrameWork.Audio;
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
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        AttackStateData attackData = AttackStateData.Create();
        attackData.dir = m_Owner.dir;
        attackData.skillID = m_SkillData.Id;
        attackData.animName = m_SkillData.AnimationName;
        attackData.animSpeed = m_SkillData.AnimSpeed;
        attackData.animTime = m_SkillData.AnimTime;
        attackData.canChangeDir = m_SkillData.CanChangeDir;

        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnAttackMsg(attackData);

        ReferencePool.ReleaseReference(attackData);
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

        if (m_Owner.currCtrl.isHitSuccess)
        {
            if (m_SkillData.IsInEffectPlaySound)
            {
                AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, "Sound/" + soundName);
            }
        }
        else
        {
            AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, "Sound/" + soundName);
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