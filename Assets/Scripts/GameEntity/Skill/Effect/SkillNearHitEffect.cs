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
                CameraMgr.Ins.Shake();
            }
        }

        m_Owner.OnHitEnd(m_SkillData, hurtTarget);

        Complete();
    }

    private bool Hit(ICanBeHit hit)
    {
        if (hit != null && hit.CanBeHit)
        {
            float dir = (hit as BaseSceneObject).Pos.x - m_Owner.Pos.x >= 0 ? 1 : -1;
            int defenseValue = 0;
            bool isBoss = false;
            bool isCritical = false;

            if (m_SkillEffect.ForceType == SkillConfigData.SkillAddForceType.SelfDir)
            {
                dir = m_Owner.Dir;
            }

            if (hit is BaseRole)
            {
                defenseValue = (hit as BaseRole).DefenseValue;
            }

            if(m_Owner is BaseEnemy)
            {
                isBoss = (m_Owner as BaseEnemy).IsBoss;
            }

            HurtData hurtData = HurtData.Create();
            hurtData.Id = m_SkillData.Id;
            hurtData.SkillExp = m_SkillData.EXP;
            hurtData.AttackerDir = m_Owner.Dir;
            hurtData.AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * dir, m_SkillEffect.AddTargetForce.y);
            hurtData.AttackerPos = m_Owner.Pos;
            hurtData.CanBeDefense = m_SkillEffect.CanBeDefense;
            hurtData.IsSwoon = m_SkillEffect.IsSmoon;
            hurtData.AttackerId = m_Owner.Id;
            hurtData.AttackValue = SkillFactory.CacDamage(m_Owner.AttackValue, defenseValue, m_Owner.CriticalValue, m_SkillEffect.DamageMulity, out isCritical);
            hurtData.IsCritical = isCritical;
            hurtData.HurtSound = m_SkillData.HurtSound;
            hurtData.HurtAnim = string.Empty;
            hurtData.IsGroundHurt = m_SkillEffect.IsOnGroundHurt;
            hurtData.IsBoss = isBoss;
            hit.OnHurtMsg(hurtData);

            return !hit.IsDead;
        }

        return false;
    }
}