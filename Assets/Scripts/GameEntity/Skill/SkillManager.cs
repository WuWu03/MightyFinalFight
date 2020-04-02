using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime
{
    public class SkillManager
    {
        public SkillManager(BaseAvatar owner ,int[] skillIDs)
        {
            m_Owner = owner;
            m_SkillDeployers = new SkillDeployer[skillIDs.Length];
            for (int i = 0; i < m_SkillDeployers.Length; i++)
            {
                m_SkillDeployers[i] = new SkillDeployer(skillIDs[i]);
            }
        }

        public void ExcuteSkill(int id)
        {
            SkillDeployer deployer = null;
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
                deployer.DeployeSkill(m_Owner);
                m_CurrSkillDeployer = deployer;
            }
            else Logger.LogError("Skill not found id:", id);
        }

        public void Update()
        {
            if (m_CurrSkillDeployer == null) return;
            if (m_CurrSkillDeployer.IsAllComplete())
            {
                m_CurrSkillDeployer = null;
                if (!m_Owner.IsAnyState(typeof(RoleAttack), typeof(RoleJumpAttack)))
                    m_Owner.FsmMachine.ChangeDefaultState();
            }
        }

        public void Destroy()
        {
            m_Owner = null;
            m_SkillDeployers = null;
            m_CurrSkillDeployer = null;
        }

        private BaseAvatar m_Owner = null;
        private SkillDeployer m_CurrSkillDeployer = null;
        private SkillDeployer[] m_SkillDeployers = null;
    }
}
