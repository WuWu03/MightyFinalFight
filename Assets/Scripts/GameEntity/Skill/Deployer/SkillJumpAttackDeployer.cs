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
        if (!m_IsOnGround)
        {
            return;
        }

        m_IsOnGround = false;
        m_CanEffect = true;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);

        SkillStateData skillStateData = SkillStateData.Create();
        skillStateData.skillID = m_SkillData.id;
        skillStateData.animName = m_SkillData.AnimationName;
        skillStateData.animSpeed = m_SkillData.AnimSpeed;
        skillStateData.animTime = m_SkillData.AnimTime;
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
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Just && !m_Owner.isPause)
        {
            if (m_CanEffect)
            {
                if (m_Owner.isHitSuccess)
                {
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
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
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
        if (!m_Owner.isHitSuccess)
        {
            m_CanEffect = true;
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
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    private bool m_CanEffect = false;
    private bool m_IsOnGround = true;
}