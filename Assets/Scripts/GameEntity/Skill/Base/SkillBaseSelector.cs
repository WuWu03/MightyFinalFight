using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBaseSelector : SkillBase, ISkillSelector
{
    public SkillBaseSelector(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) 
    {
        m_ListTargets = new List<ICanBeHit>();
    }

    public abstract List<ICanBeHit> GetTargets();

    public abstract List<GameObject> GetTargetsObj();

    public abstract void Reset();
    public abstract void Exit();

    protected List<ICanBeHit> m_ListTargets = null;
}
