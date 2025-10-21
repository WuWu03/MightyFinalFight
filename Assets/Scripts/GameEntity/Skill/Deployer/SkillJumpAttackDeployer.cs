using DragonBones;
using GameFrameWork.Utils;

public class SkillJumpAttackDeployer : SkillBaseDeployer
{
    private int m_TriggerTime;
    private bool m_CanEffect;
    private bool m_IsOnGround = true;
    
    public SkillJumpAttackDeployer(int skillID, BaseRole owner) : base(skillID, owner)
    {

    }

    public override void DeploySkill()
    {
        m_IsOnGround = false;
        m_CanEffect = true;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        SkillStateArg skillStateArg = SkillStateArg.Create();
        skillStateArg.skillID = mSkillData.id;
        skillStateArg.animName = mSkillData.AnimationName;
        skillStateArg.animSpeed = mSkillData.AnimSpeed;
        skillStateArg.animTime = mSkillData.AnimTime;
        skillStateArg.dir = m_Owner.dir;
        skillStateArg.canChangeDir = false;
        m_Owner.onGroundEvent += OnGroundEvent;
        m_Owner.onDropEvent += OnDropEvent;
        m_Owner.AddAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.AddAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_Owner.SetIsBeCatch(false);
        m_Owner.OnAttackMsg(skillStateArg, true);
        skillStateArg.Release();
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
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound", StringUtil.Append(eventObject.name, ".ogg")));
    }

    private void OnDropEvent()
    {
        m_Owner.onDropEvent -= OnDropEvent;
        m_CanEffect = m_TriggerTime < mSkillData.JumpTriggerTime;

        if (m_CanEffect)
        {
            m_Owner.SetHitSuccess(false);
        }
    }

    private void OnGroundEvent()
    {
        m_Owner.onGroundEvent -= OnDropEvent;
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
        m_IsOnGround = true;
        m_CanEffect = false;
    }

    public override void Exit()
    {
        base.Exit();
        m_CanEffect = true;
        m_TriggerTime = 0;
        m_Owner.onDropEvent -= OnDropEvent;
        m_Owner.onGroundEvent -= OnGroundEvent;
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }

    protected override void OnRemoveEvent()
    {
        base.OnRemoveEvent();
        m_Owner.RemoveAnimationEvent(EventObject.FRAME_EVENT, SkillEvent);
        m_Owner.RemoveAnimationEvent(EventObject.SOUND_EVENT, SoundEvent);
    }
}