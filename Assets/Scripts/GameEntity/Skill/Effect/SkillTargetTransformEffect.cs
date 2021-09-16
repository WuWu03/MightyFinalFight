using System.Collections.Generic;
using UnityEngine;

public class SkillTargetTransformEffect : SkillBaseEffect
{
    public SkillTargetTransformEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
        List<ICanBeHit> targets = m_Owner.OnHitStart();

        if (targets == null || targets.Count < 1)
        {
            Complete();
            return;
        }

        BaseRole target = targets[0] as BaseRole;

        if (m_SkillEffect.EffectorType == SkillConfigData.SkillEffectorType.TargetPositionEffect)
        {
            float targetY = target.Pos.y;
            target.SetPos2(m_Owner.Pos.x + m_SkillEffect.MoveTarget.x * m_Owner.Dir,
                       m_Owner.Pos.y + m_SkillEffect.MoveTarget.y);
            target.UpdatePos2(target.Pos.x, targetY);
        }
        else if (m_SkillEffect.EffectorType == SkillConfigData.SkillEffectorType.TargetScaleEffect)
        {
            target.SetScale2(m_Owner.Dir * m_SkillEffect.ScaleTarget.x, m_SkillEffect.ScaleTarget.y);
        }

        if (m_SkillEffect.IsSmoon)
        {
            target.PlayAnimation(AnimName.SwoonUp);
        }

        Complete();
    }

    protected override void OnExit()
    {
        if (m_Owner is BaseHero)
        {
            BaseHero owner = m_Owner as BaseHero;

            if (owner.IsCatch)
            {
                List<ICanBeHit> targets = m_Owner.OnHitStart();
                BaseRole target = targets[0] as BaseRole;
                target.SetCatch(false);
                if (!m_SkillEffect.IsSmoon)
                    target.PlayAnimation(AnimName.Idle);
                owner.ResetCatch(false);
            }
        }
    }
}