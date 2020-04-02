
namespace Runtime
{
    public class SkillDeployer
    {
        public int SkillID { get; private set; }
        public SkillDeployer(int skillID)
        {
            SkillID = skillID;
            m_SkillData = StaticConfig.SkillConfig.GetData(skillID);
            m_SkillSelector = SkillDeployerFactory.CreateSelector(m_SkillData.SelectorType);
            m_SkillEffects = SkillDeployerFactory.CreateEffects(m_SkillData.EffectorTypes);
        }

        public virtual void DeployeSkill(BaseAvatar owner)
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
            {
                m_SkillEffects[i].Effect(owner, m_SkillData, m_SkillSelector);
            }
        }

        public virtual bool IsAllComplete()
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
            {
                if(!m_SkillEffects[i].IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        private SkillData m_SkillData = null;
        private ISkillSelector m_SkillSelector = null;
        private ISkillEffect[] m_SkillEffects = null;
    }
}
