using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBaseSelector : SkillBase, ISkillSelector
{
    public SkillBaseSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) 
    {
        m_ListTargets = new List<ICanBeHit>();
    }

    public abstract List<ICanBeHit> GetTargets();

    public abstract void Reset();
    public abstract void Exit();

    protected List<ICanBeHit> m_ListTargets = null;
}
