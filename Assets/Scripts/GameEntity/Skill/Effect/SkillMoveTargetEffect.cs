using System.Collections.Generic;
using UnityEngine;

public class SkillMoveTargetEffect : SkillBaseEffect
{
    public SkillMoveTargetEffect(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
    public override bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }

    public override void Effect(ISkillSelector selector)
    {
        m_IsCompleted = false;
        List<ICanBeHit> targets = m_Owner.OnHitStart();

        if (targets == null || targets.Count < 1)
        {
            m_IsCompleted = true;
            return;
        }

        BaseRole target = targets[0] as BaseRole;

        float targetY = target.Pos.y;
        target.SetPos2(m_Owner.Pos.x + m_SkillEffect.MoveTarget.x * m_Owner.Dir,
                   m_Owner.Pos.y + m_SkillEffect.MoveTarget.y);
        target.UpdatePos2(target.Pos.x, targetY);

        if (m_SkillEffect.IsSmoon)
        {
            target.PlayAnimation(AnimName.SwoonUp);
        }
    }

    public override void Reset()
    {
        m_IsCompleted = false;
        Exit();
    }

    public override void Exit()
    {
        if(m_Owner is BaseHero)
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

    public override void Update(ISkillSelector selector)
    {
        if(m_Owner.IsPlayComplete())
        {
            m_IsCompleted = true;
        }
    }
}