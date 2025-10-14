using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.Utils;
using static SkillConfigData;

public class SkillSkillAttackDeployer : SkillBaseDeployer
{
    public SkillSkillAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner) { }

    public override void DeploySkill()
    {
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        SkillStateData skillData = SkillStateData.Create();
        skillData.skillID = mSkillData.id;
        skillData.animName = mSkillData.AnimationName;
        skillData.animTime = mSkillData.AnimTime;
        skillData.animSpeed = mSkillData.AnimSpeed;
        skillData.canChangeDir = mSkillData.CanChangeDir;
        skillData.canMove = mSkillData.CanMove;

        if (mSkillData.TriggerType != SkillTriggerType.Animtion)
        {
            m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
            m_Owner.OnSkillMsg(skillData);
            base.DeploySkill();
            skillData.Release();
            return;
        }

        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.OnSkillMsg(skillData);
        skillData.Release();
    }

    public override bool IsAllComplete()
    {
        bool isComplete = base.IsAllComplete();

        if (isComplete)
        {
            m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);

            if (!mSkillData.IsInEffectPlaySound)
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
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound", StringUtil.Append(eventObject.name, ".ogg")));
    }

    public override void Exit()
    {
        base.Exit();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        if (!mSkillData.IsInEffectPlaySound)
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