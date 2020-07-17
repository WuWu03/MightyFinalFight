using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase
{
    public SkillBase(SkillData skillData,BaseRole owner,int effectIndex)
    {
        m_SkillData = skillData;
        m_Owner = owner;
        m_SkillEffect = skillData.SkillEffects[effectIndex];
    }

    protected SkillData.SkillEffect m_SkillEffect = null;
    protected SkillData m_SkillData = null;
    protected BaseRole m_Owner = null;
}
