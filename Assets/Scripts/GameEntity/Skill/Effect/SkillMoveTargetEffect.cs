using FrameWork.GameEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<ICanBeHit> targets = m_Owner.OnHitStart();

        if (targets == null || targets.Count < 1)
        {
            m_IsCompleted = true;
            return;
        }

        BaseRole bo = targets[0] as BaseRole;

        float targetY = bo.Pos.y;
        bo.SetPos2(m_Owner.Pos.x + m_SkillEffect.MoveTarget.x * m_Owner.Dir,
                   m_Owner.Pos.y + m_SkillEffect.MoveTarget.y);
        bo.UpdatePos2(bo.Pos.x, targetY);

        if (m_SkillEffect.IsSmoon)
        {   
            bo.PlayAnimation(AnimName.SmoonUp);
        }

        m_IsCompleted = true;
    }

    public override void Reset()
    {
        m_IsCompleted = false;
    }

    public override void Exit()
    {
        if(m_Owner is BaseHero)
        {
            BaseHero bh = m_Owner as BaseHero;

            if (bh.IsCatch)
            {
                List<ICanBeHit> targets = m_Owner.OnHitStart();
                (targets[0] as BaseRole).PlayAnimation(AnimName.Idle);
                bh.ResetCatch(false);
            }
        }

        m_IsCompleted = false;
    }

    public override void Update()
    {

    }
}