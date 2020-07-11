using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SkillSubHPEffect : ISkillEffect
{
    public bool IsCompleted
    {
        get
        {
            return m_IsComplete;
        }
    }

    public int Index
    {
        get;
        set;
    }

    public void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector)
    {
        if (m_IsComplete) return;

        foreach (Match m in m_Regex.Matches(skillData.SkillEffects[Index].Args))
        {
            string[] str = m.Value.Split(':');
            owner.SubHealth(int.Parse(str[1]));
        }

        m_IsComplete = true;
    }

    public void Reset()
    {
        
    }

    public void Exit()
    {
        m_IsComplete = false;
    }

    private Regex m_Regex = new Regex(@"SubHP:[0-9]+");
    private bool m_IsComplete = false;
}
