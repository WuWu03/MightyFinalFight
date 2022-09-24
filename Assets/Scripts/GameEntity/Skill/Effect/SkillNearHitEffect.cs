using GameFrameWork;
using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillConfigData m_SkillData, BaseRole owner, int effectIndex) : base(m_SkillData, owner, effectIndex)
    {

    }

    public override void Effect(ISkillSelector skillSelector)
    {
        if (m_SkillEffect.Args == "HeroAttackEnd")
        {
            m_Owner.SetDefaultState<HeroAttackEnd>();
        }

        bool hurtTarget = false;
        List<ICanBeHit> targets = m_Owner.OnHitStart();

        if (targets == null)
        {
            targets = skillSelector.GetTargets();
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (Hit(targets[i]))
            {
                hurtTarget = true;
            }
        }

        if (hurtTarget)
        {
            if (m_SkillEffect.IsShakeCamera)
            {
                CameraMgr.instance.Shake();
            }
        }

        m_Owner.OnHitEnd(m_SkillData, hurtTarget);

        Complete();
    }

    private bool Hit(ICanBeHit hit)
    {
        return SkillFactory.SkillHit(hit, m_Owner, m_SkillData, m_SkillEffect);
    }
}