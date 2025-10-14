using System.Collections.Generic;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector skillSelector)
    {
        m_Targets = m_Owner.OnHitStart();

        if (m_Targets == null || m_Targets.Count < 1)
        {
            m_Targets = skillSelector.GetTargets();
        }

        if (m_Targets == null || m_Targets.Count < 1)
        {
            Complete();
            return;
        }

        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (HurtTarget(m_Targets[i]))
            {
                m_IsHurtTarget = true;
            }
        }
        
        m_Owner.OnHitEnd(mSkillData, m_IsHurtTarget);
        Complete();
    }
    
    protected override void OnComplete()
    {
        base.OnComplete();
        m_IsHurtTarget = false;
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_IsHurtTarget = false;
        m_Targets = null;
    }

    private bool HurtTarget(ICanBeHit canBeHit)
    {
        return SkillUtil.SkillHit(canBeHit, m_Owner, mSkillData, m_SkillEffect);
    }

    private bool m_IsHurtTarget = false;
    private List<ICanBeHit> m_Targets = null;
}