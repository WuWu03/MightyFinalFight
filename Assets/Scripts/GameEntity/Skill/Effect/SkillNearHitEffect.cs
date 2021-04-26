using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillData m_SkillData, BaseRole owner, int effectIndex) : base(m_SkillData, owner, effectIndex)
    {
        m_HurtData = new HurtData();
    }

    public override bool IsCompleted
    {
        get
        {
            if (m_SkillData.TriggerType == SkillData.SkillTriggerType.Animtion)
            {
                m_IsCompleted = m_Owner.IsPlayComplete();
            }

            return m_IsCompleted;
        }
    }


    public override void Effect(ISkillSelector skillSelector)
    {
        m_IsCompleted = false;

        bool hurtTarget = false;
        List<ICanBeHit> targets = m_Owner.OnHitStart();
        
        if(targets == null)
        {
            targets = skillSelector.GetTargets();
        }
        
        for (int i = 0; i < targets.Count; i++)
        {
            if(Hit(targets[i]))
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
        m_IsCompleted = true;
    }

    private bool Hit(ICanBeHit hit)
    {
        if (hit != null && hit.CanBeHit)
        {
            float dir = (hit as BaseSceneObject).Pos.x - m_Owner.Pos.x >= 0 ? 1 : -1;
            if(m_SkillEffect.ForceType == SkillData.SkillAddForceType.SelfDir)
            {
                dir = m_Owner.Dir;
            }

            m_HurtData.ID = m_SkillData.ID;
            m_HurtData.SkillExp = m_SkillData.EXP;
            m_HurtData.AttackerDir = m_Owner.Dir;
            m_HurtData.AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * dir, m_SkillEffect.AddTargetForce.y);
            m_HurtData.AttackerPos = m_Owner.Pos;
            m_HurtData.CanBeDefense = m_SkillEffect.CanBeDefense;
            m_HurtData.IsSwoon = m_SkillEffect.IsSmoon;
            m_HurtData.AttackerID = m_Owner.ID;
            m_HurtData.AttackValue = 1;
            m_HurtData.HurtSound = m_SkillData.HurtSound;
            m_HurtData.HurtAnim = string.Empty;
            m_HurtData.IsGroundHurt = m_SkillEffect.IsOnGroundHurt;
            hit.OnHurtMsg(m_HurtData);           
            return true;
        }

        return false;
    }

    public override void Reset()
    {
        m_IsCompleted = false;
        if (!m_SkillEffect.IsOnGroundHurt)
            m_HurtData.Clear();
    }

    public override void Exit()
    {

    }

    public override void Update(ISkillSelector selector)
    {

    }

    private HurtData m_HurtData = null;
}