using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SkillSubHPEffect : SkillBaseEffect
{
    public SkillSubHPEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
        if (!m_Owner.HitSuccess) return;

        foreach (Match m in m_Regex.Matches(m_SkillEffect.Args))
        {
            m_Owner.Attribute.SubHealth(int.Parse(m.Groups[2].Value));
        }

        Complete();
    }

    private Regex m_Regex = new Regex(@"(SubHP:)([0-9]+)");
}
