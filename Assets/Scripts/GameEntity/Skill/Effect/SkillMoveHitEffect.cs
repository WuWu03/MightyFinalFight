using GameFrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveHitEffect : SkillBaseEffect
{
    public SkillMoveHitEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
        m_HasEffect = true;
        m_StartPos = m_Owner.transform.localPosition;
        m_Owner.SetVelocity(m_SkillEffect.AddSelfVelocity.x * m_Owner.Dir, m_SkillEffect.AddSelfVelocity.y);
        m_Owner.SetDrag(m_SkillEffect.AddSelfDrag);
        m_Owner.SetGravityScale(m_SkillEffect.Gravity);

        if (m_SkillEffect.Args == "OnGroundPickUp")
        {
            m_Owner.OnGroundEvent.AddListener(OnGround);
        }
    }

    private void OnGround()
    {
        m_Owner.SetDefaultState<HeroPickUp>();
    }

    protected override void OnComplete()
    {
        m_Owner.ResetRigidbody(false);
        m_HasEffect = false;
    }

    protected override void OnUpdate(ISkillSelector selector)
    {
        if (!m_HasEffect)
        {
            return;
        }

        if (m_Owner.Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1)
        {
            Complete();
            return;
        }

        m_Owner.UpdatePosX(m_Owner.transform.localPosition.x);
        CheckAttack(selector);

        if (m_SkillEffect.MoveDistance > 0)
        {
            float dis = Mathf.Abs(m_SkillEffect.MoveDistance - Vector3.Distance(m_StartPos, m_Owner.transform.localPosition));
            if (dis <= 0.1f)
            {
                Complete();
            }
        } 
    }

    private void CheckAttack(ISkillSelector selector)
    {
        m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);
        List<ICanBeHit> targets = selector.GetTargets();

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].CanBeHit)
            {
                int defenseValue = 0;
                bool isCritical = false;

                if (targets[i] is BaseRole)
                {
                    defenseValue = (targets[i] as BaseRole).DefenseValue;
                }

                HurtData hurtData = HurtData.Create();
                hurtData.AttackerId = m_Owner.Id;
                hurtData.AttackerDir = m_Owner.Dir;
                hurtData.AttackerPos = m_Owner.Pos;
                hurtData.AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * m_Owner.Dir, m_SkillEffect.AddTargetForce.y);
                hurtData.IsSwoon = m_SkillEffect.IsSmoon;
                hurtData.AttackValue = SkillFactory.CacDamage(m_Owner.AttackValue, defenseValue, m_Owner.CriticalValue, m_SkillEffect.DamageMulity, out isCritical);
                hurtData.IsCritical = isCritical;
                targets[i].OnHurtMsg(hurtData);

                if (m_SkillEffect.HitOne)
                {
                    Complete();
                    break;
                }
            }
        }
    }

    private bool m_HasEffect = false;
    private Vector3 m_StartPos = Vector3.zero;
}