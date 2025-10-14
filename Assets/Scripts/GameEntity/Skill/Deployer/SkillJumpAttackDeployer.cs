using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.Utils;

public class SkillJumpAttackDeployer : SkillBaseDeployer
{
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {

    }

    public override void DeploySkill()
    {
        m_IsOnGround = false;
        m_CanEffect = true;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        SkillStateData skillStateData = SkillStateData.Create();
        skillStateData.skillID = mSkillData.id;
        skillStateData.animName = mSkillData.AnimationName;
        skillStateData.animSpeed = mSkillData.AnimSpeed;
        skillStateData.animTime = mSkillData.AnimTime;
        skillStateData.dir = m_Owner.dir;
        skillStateData.canChangeDir = false;

        m_Owner.onGroundEvent.AddListener(OnGroundEvent);
        m_Owner.onDropEvent.AddListener(OnDropEvent);
        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.SetIsBeCatch(false);
        m_Owner.OnAttackMsg(skillStateData, true);
        skillStateData.Release();
    }

    public override bool IsAllComplete()
    {
        if (m_IsOnGround)
        {
            m_CanEffect = false;
            m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        }

        return m_IsOnGround;
    }

    public override void Update()
    {
        if (mSkillData.TriggerType == SkillConfigData.SkillTriggerType.Just)
        {
            if (m_CanEffect)
            {
                if (m_Owner.isHitSuccess)
                {
                    m_TriggerTime++;
                    m_CanEffect = false;
                    return;
                }

                base.DeploySkill();
            }
        }

        base.Update();
    }

    private void SkillEvent(string type, EventObject eventObject)
    {
        if (mSkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
        {
            base.DeploySkill();
        }
    }

    private void SoundEvent(string type, EventObject eventObject)
    {
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound", StringUtil.Append(eventObject.name, ".ogg")));
    }

    private void OnDropEvent()
    {
        m_Owner.onDropEvent.RemoveListener(OnDropEvent);
        m_CanEffect = m_TriggerTime < mSkillData.JumpTriggerTime;

        if (m_CanEffect)
        {
            m_Owner.SetHitSuccess(false);
        }
    }

    private void OnGroundEvent()
    {
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_IsOnGround = true;
        m_CanEffect = false;
    }

    public override void Exit()
    {
        base.Exit();
        m_CanEffect = true;
        m_TriggerTime = 0;
        m_Owner.onDropEvent.RemoveListener(OnDropEvent);
        m_Owner.onGroundEvent.RemoveListener(OnGroundEvent);
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private int m_TriggerTime = 0;
    private bool m_CanEffect = false;
    private bool m_IsOnGround = true;
}