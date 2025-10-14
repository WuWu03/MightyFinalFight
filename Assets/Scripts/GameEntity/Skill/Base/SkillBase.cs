public abstract class SkillBase
{
    public SkillBase(SkillConfigData skillData, BaseRole owner, int effectIndex)
    {
        mSkillData = skillData;
        m_Owner = owner;
        m_SkillEffect = skillData.SkillEffects[effectIndex];
    }

    protected SkillConfigData.SkillEffect m_SkillEffect = null;
    protected SkillConfigData mSkillData = null;
    protected BaseRole m_Owner = null;
}
