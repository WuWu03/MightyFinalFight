public abstract class SkillBase
{
    public SkillBase(SkillConfigData skillData, BaseRole owner, int effectIndex)
    {
        m_SkillData = skillData;
        m_Owner = owner;
        m_SkillEffect = skillData.SkillEffects[effectIndex];
    }

    protected SkillConfigData.SkillEffect m_SkillEffect = null;
    protected SkillConfigData m_SkillData = null;
    protected BaseRole m_Owner = null;
}
