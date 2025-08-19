using System.Collections.Generic;
using UnityEngine;

public class SkillTargetTransformEffect : SkillBaseEffect
{
    public SkillTargetTransformEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) { }

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
            target.SetPosXY(m_Owner.pos.x + m_SkillEffect.MoveTarget.x * m_Owner.dir, m_Owner.pos.y + m_SkillEffect.MoveTarget.y, false);
            //target.UpdatePosXY(target.pos.x, target.pos.y);
        }
        else if (m_SkillEffect.EffectorType == SkillConfigData.SkillEffectorType.TargetScaleEffect)
        {
            target.SetScale2(m_Owner.dir * m_SkillEffect.ScaleTarget.x, m_SkillEffect.ScaleTarget.y);
        }

        if (m_SkillEffect.IsSmoon)
        {
            target.PlayAnimation(AnimName.SwoonUp);
        }

        Complete();
    }

    protected override void OnExit()
    {
        //if (m_Owner is BaseHero)
        //{
        //    BaseHero owner = m_Owner as BaseHero;

        //    if (owner.isCatching)
        //    {
        //        List<ICanBeHit> targets = m_Owner.OnHitStart();
        //        BaseRole target = targets[0] as BaseRole;
        //        target.SetIsBeCatch(false);

        //        if (!m_SkillEffect.IsSmoon)
        //        {
        //            target.PlayAnimation(AnimName.Idle);
        //        }

        //        owner.ResetCatch(false);
        //    }
        //}
    }
}