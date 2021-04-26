using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBaseEffect : SkillBase, ISkillEffect
{
    public SkillBaseEffect(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
    public virtual bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }

    public abstract void Effect(ISkillSelector selector);
    public abstract void Update(ISkillSelector selector);
    public abstract void Exit();
    public abstract void Reset();

    protected bool m_IsCompleted = false;
}
