using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SkillSubHPEffect : SkillBaseEffect
{
    public SkillSubHPEffect(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
    public override bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }


    public override void Effect(ISkillSelector selector)
    {
        if (m_IsCompleted) return;
        if (!m_Owner.HitSuccess) return;

        foreach (Match m in m_Regex.Matches(m_SkillEffect.Args))
        {
            m_Owner.SubHealth(int.Parse(m.Groups[2].Value));
        }

        m_IsCompleted = true;
    }

    public override void Reset()
    {
        
    }

    public override void Exit()
    {
        m_IsCompleted = false;
    }

    public override void Update()
    {

    }

    private Regex m_Regex = new Regex(@"(SubHP:)([0-9]+)");
}
