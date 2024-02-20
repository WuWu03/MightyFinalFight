using GameFrameWork;
public class SkillManager
{
    public SkillManager(BaseRole owner, int[] skillIDs)
    {
        m_Owner = owner;
        m_SkillDeployers = new SkillBaseDeployer[skillIDs.Length];

        for (int i = 0; i < m_SkillDeployers.Length; i++)
        {
            m_SkillDeployers[i] = SkillFactory.CreateDeployer(skillIDs[i], owner);
        }
    }

    public void DeploySkill(int id)
    {
        if (m_CurrSkillDeployer != null && m_CurrSkillDeployer.skillId == id)
        {
            return;
        }

        SkillBaseDeployer deployer = null;

        for (int i = 0; i < m_SkillDeployers.Length; i++)
        {
            if (m_SkillDeployers[i].skillId.Equals(id))
            {
                deployer = m_SkillDeployers[i];
                break;
            }
        }

        if (deployer != null)
        {
            RemoveAllEvent();

            if (!SkillUtil.CheckStatus(deployer.skillData.SkillPrevConditions, m_Owner))
            {
                return;
            }

            deployer.AddEvent();
            deployer.DeploySkill();
            m_CurrSkillDeployer = deployer;
        }
        else
        {
            Log.LogError("Skill not found id:", id);
        }
    }

    public void Update()
    {
        if (m_CurrSkillDeployer == null)
        {
            return;
        }

        m_CurrSkillDeployer.Update();

        if (m_CurrSkillDeployer.IsAllComplete())
        {
            ExitSkill();

            if (m_Owner.canChangeDefaultState && !m_Owner.isAddGroundForce && !m_Owner.isDrop)
            {
                m_Owner.ChangeDefaultState();
            }
        }
    }

    public bool IsInSkill()
    {
        if (m_CurrSkillDeployer == null)
        {
            return false;
        }

        return !m_CurrSkillDeployer.IsAllComplete();
    }

    public bool IsCurrSkill(int id)
    {
        if (m_CurrSkillDeployer == null)
        {
            return false;
        }

        if (!m_CurrSkillDeployer.skillId.Equals(id))
        {
            return false;
        }

        return true;
    }

    public bool IsSkillComplete(int id)
    {
        if (m_CurrSkillDeployer == null)
        {
            return false;
        }

		if (!m_CurrSkillDeployer.skillId.Equals(id))
        {
            return false;
        }

        return m_CurrSkillDeployer.IsAllComplete();
    }

    public void ExitSkill()
    {
        if (m_CurrSkillDeployer == null)
        {
            return;
        }

        m_CurrSkillDeployer.RemoveEvent();
        m_CurrSkillDeployer.Exit();
        m_CurrSkillDeployer = null;
    }

    public void Release()
    {
        m_Owner = null;
        m_SkillDeployers = null;
        m_CurrSkillDeployer = null;
    }

    private void RemoveAllEvent()
    {
        for (int i = 0; i < m_SkillDeployers.Length; i++)
        {
            m_SkillDeployers[i].RemoveEvent();
        }
    }

    private BaseRole m_Owner = null;
    private SkillBaseDeployer m_CurrSkillDeployer = null;
    private SkillBaseDeployer[] m_SkillDeployers = null;
}