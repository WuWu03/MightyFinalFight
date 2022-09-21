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
        m_Owner.SetVelocity(m_SkillEffect.AddSelfVelocity.x * m_Owner.dir, m_SkillEffect.AddSelfVelocity.y);
        m_Owner.SetDrag(m_SkillEffect.AddSelfDrag);
        m_Owner.SetGravityScale(m_SkillEffect.Gravity);

        if (m_SkillEffect.Args == "HeroAttackEnd")
        {
            m_Owner.onGroundEvent.AddListener(OnGround);
        }
    }

    private void OnGround()
    {
        m_Owner.SetDefaultState<HeroAttackEnd>();
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

        if (m_Owner.rigidbody2D.velocity.sqrMagnitude <= 0.1 * 0.1)
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
        m_Owner.UpdatePosXY(m_Owner.transform.localPosition.x, m_Owner.pos.y);
        List<ICanBeHit> targets = selector.GetTargets();

        for (int i = 0; i < targets.Count; i++)
        {
            if (SkillFactory.SkillHit(targets[i], m_Owner, m_SkillData, m_SkillEffect) && m_SkillEffect.HitOne)
            {
                Complete();
                return;
            }
        }
    }

    private bool m_HasEffect = false;
    private Vector3 m_StartPos = Vector3.zero;
}