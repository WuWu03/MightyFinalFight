using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBaseEffect : SkillBase, ISkillEffect
{
    public SkillBaseEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }

    public abstract void Effect(ISkillSelector selector);

    public void Update(ISkillSelector selector)
    {
        OnUpdate(selector);
    }
 
    public void Complete()
    {
        m_IsCompleted = true;
        OnComplete();
    }

    public void Exit()
    {
        OnExit();
    }

    public void Reset()
    {
        m_IsCompleted = false;
        OnReset();
    }

    protected virtual void OnUpdate(ISkillSelector selector) { }
    protected virtual void OnReset() { }
    protected virtual void OnComplete() { }
    protected virtual void OnExit() { }

    private bool m_IsCompleted = false;
}
