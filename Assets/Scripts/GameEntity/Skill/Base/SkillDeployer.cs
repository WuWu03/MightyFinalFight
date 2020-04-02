
namespace Runtime
{
    public abstract class SkillDeployer
    {
        public int SkillID { get; private set; }
        public SkillDeployer(int skillID,BaseRole owner)
        {
            SkillID = skillID;
            m_Owner = owner;
            m_SkillData = StaticConfig.SkillConfig.GetData(skillID);
            m_SkillSelector = SkillDeployerFactory.CreateSelector(m_SkillData.SelectorType);
            m_SkillEffects = SkillDeployerFactory.CreateEffects(m_SkillData.EffectorTypes);
        }

        public virtual void DeploySkill()
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
            {
                m_SkillEffects[i].Effect(m_Owner, m_SkillData, m_SkillSelector);
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

        public virtual void Update() { }

        protected BaseRole m_Owner = null;
        protected SkillData m_SkillData = null;
        private ISkillSelector m_SkillSelector = null;
        private ISkillEffect[] m_SkillEffects = null;
    }
}
