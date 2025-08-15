using System.Text.RegularExpressions;

public class SkillSubHPEffect : SkillBaseEffect
{
    public SkillSubHPEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex)
    {
        m_Regex = new(@"(SubHP:)([0-9]+)");
    }

    public override void Effect(ISkillSelector selector)
    {
        if (!m_Owner.isHitSuccess)
        {
            return;
        }

        foreach (Match m in m_Regex.Matches(m_SkillEffect.Args))
        {
            m_Owner.entityAttribute.SubHealth(int.Parse(m.Groups[2].Value));
        }

        Complete();
    }

    private Regex m_Regex = null;
}
