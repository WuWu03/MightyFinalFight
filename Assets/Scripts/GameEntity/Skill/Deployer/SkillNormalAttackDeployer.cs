using FrameWork.Sound;
using System.Diagnostics;

public class SkillNormalAttackDeployer : SkillDeployer
{
    public SkillNormalAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {
        m_AttackMsgData = new AttackData();
    }

    public override void DeploySkill()
    {
        m_AttackMsgData.Dir = m_Owner.Dir;
        m_AttackMsgData.CanChangeDir = true;
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
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
        base.DeploySkill();
    }

    private void SoundEvent(string type, DragonBones.EventObject eventObject)
    {
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", eventObject.name);
        m_Owner.ActorAnimator.RemoveEventListener(DragonBones.EventObject.SOUND_EVENT, SoundEvent);
    }

    private AttackData m_AttackMsgData = null;
}