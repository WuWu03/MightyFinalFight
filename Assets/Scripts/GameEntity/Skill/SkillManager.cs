using UnityEngine;

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
        if (m_CurrSkillDeployer != null && m_CurrSkillDeployer.SkillID == id) return;
        SkillBaseDeployer deployer = null;
        for (int i = 0; i < m_SkillDeployers.Length; i++)
        {
            if (m_SkillDeployers[i].SkillID.Equals(id))
            {
                deployer = m_SkillDeployers[i];
                break;
            }
        }

        if (deployer != null)
        {
            if (!SkillFactory.CheckStatus(deployer.SkillData.SkillPrevConditions, m_Owner))
            {
                return;
            }

            deployer.DeploySkill();
            m_CurrSkillDeployer = deployer;
        }
        else GameFrameWork.Log.Debugger.LogError("Skill not found id:", id);
    }

    public void Update()
    {
        if (m_CurrSkillDeployer == null) return;

        m_CurrSkillDeployer.Update();

        if (m_CurrSkillDeployer.IsAllComplete())
        {
            m_CurrSkillDeployer.OnExit();
            m_CurrSkillDeployer = null;
            if (m_Owner.CanChangeDefaultState)
                m_Owner.FsmMachine.ChangeDefaultState();
        }
    }

    public void ExitSkill()
    {
        if (m_CurrSkillDeployer == null) return;
        m_CurrSkillDeployer.OnExit();
        m_CurrSkillDeployer = null;
    }

    public void Release()
    {
        m_Owner = null;
        m_SkillDeployers = null;
        m_CurrSkillDeployer = null;
    }

    private BaseRole m_Owner = null;
    private SkillBaseDeployer m_CurrSkillDeployer = null;
    private SkillBaseDeployer[] m_SkillDeployers = null;
}