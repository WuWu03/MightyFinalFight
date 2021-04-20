using GameFrameWork.Sound;
using System.Collections.Generic;
using UnityEngine;

public class SkillNormalAttackDeployer : SkillBaseDeployer
{
    public SkillNormalAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_AttackMsgData = new AttackData();
        m_QueueSound = new Queue<string>();
    }

    public override void DeploySkill()
    {
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);

        m_AttackMsgData.Dir = m_Owner.Dir;
        m_AttackMsgData.CanChangeDir = m_SkillData.CanChangeDir;
        m_AttackMsgData.AnimationName = m_SkillData.AnimationName;
        m_AttackMsgData.AnimSpeed = m_SkillData.AnimSpeed;
        m_AttackMsgData.AnimTime = m_SkillData.AnimTime;

        if (m_SkillData.DeployeType == SkillData.SkillDeployeType.Just)
        {
            m_AttackMsgData.AddSelfForce = m_SkillData.SkillEffects[0].AddSelfForce;
        }

        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.AddEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);

        m_Owner.OnAttackMsg(m_AttackMsgData);
    }

    private void SkillEvent(string type, DragonBones.EventObject eventObject)
    {
        if(CurrEffect.AddSelfForce != Vector2.zero)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Owner.Rigidbody.AddForce(new Vector2(CurrEffect.AddSelfForce.x * m_Owner.Dir, CurrEffect.AddSelfForce.y));
        }
        base.DeploySkill();
        RealPlaySound();
    }

    protected override void OnEffectComplete()
    {
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

    private void SoundEvent(string type, DragonBones.EventObject eventObject)
    {
        m_QueueSound.Enqueue(eventObject.name);
    }

    private void RealPlaySound()
    {
        if (m_QueueSound.Count < 1) return;
        string soundName = m_QueueSound.Dequeue();

        if (m_Owner.HitSuccess)
        {
            if (m_SkillData.IsInEffectPlaySound)
                SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", soundName);
        }
        else
            SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", soundName);
    }

    public override void OnExit()
    {
        base.OnExit();
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

    private Queue<string> m_QueueSound = null;
    private AttackData m_AttackMsgData = null;
}