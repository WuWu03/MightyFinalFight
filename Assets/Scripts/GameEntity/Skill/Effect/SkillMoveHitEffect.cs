using GameFrameWork;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveHitEffect : SkillBaseEffect
{
    public SkillMoveHitEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
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
        m_HasEffect = true;
        m_StartPos = m_Owner.transform.localPosition;
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.Rigidbody.velocity = new Vector2(m_SkillEffect.AddSelfVelocity.x * m_Owner.Dir, m_SkillEffect.AddSelfVelocity.y);
        m_Owner.Rigidbody.drag = m_SkillEffect.AddSelfDrag;
        m_Owner.Rigidbody.gravityScale = m_SkillEffect.Gravity;

        if (m_SkillEffect.Args == "OnGroundPickUp")
            m_Owner.OnGroundEvent.AddListener(OnGround);
    }

    private void OnGround()
    {
        m_Owner.FsmMachine.SetDefaultState<HeroPickUp>();
    }

    private void Complete()
    {
        m_Owner.Rigidbody.velocity = Vector2.zero;
        m_Owner.Rigidbody.gravityScale = 1.0f;
        m_Owner.Rigidbody.drag = 0f;
        m_IsCompleted = true;
        m_HasEffect = false;
    }

    public override void Reset()
    {
        m_IsCompleted = false;
    }

    public override void Exit()
    {
   
    }

    public override void Update(ISkillSelector selector)
    {
        if(!m_HasEffect)
        {
            return;
        }

        CheckAttack(selector);

        if (m_SkillEffect.MoveDistance < 0)
        {
            if (m_Owner.Rigidbody.velocity.sqrMagnitude <= 0.1 * 0.1)
                Complete();
            return;
        }
        else
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
                HurtData hurtData = HurtData.Create();
                hurtData.AttackerID = m_Owner.ID;
                hurtData.AttackerDir = m_Owner.Dir;
                hurtData.AttackerPos = m_Owner.Pos;
                hurtData.AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * m_Owner.Dir, m_SkillEffect.AddTargetForce.y);
                hurtData.IsSwoon = m_SkillEffect.IsSmoon;
                hurtData.AttackValue = 1;
                
                targets[i].OnHurtMsg(hurtData);

                if(m_SkillEffect.HitOne)
                {
                    m_IsCompleted = true;
                    break;
                }
            }
        }
    }

    private bool m_HasEffect = false;
    private Vector3 m_StartPos = Vector3.zero;
}