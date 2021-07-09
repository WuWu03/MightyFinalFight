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

    public override bool IsCompleted
    {
        get
        {
            if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
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

            if(m_SkillEffect.ForceType == SkillConfigData.SkillAddForceType.SelfDir)
            {
                dir = m_Owner.Dir;
            }

            HurtData hurtData = HurtData.Create();
            hurtData.Id = m_SkillData.ID;
            hurtData.SkillExp = m_SkillData.EXP;
            hurtData.AttackerDir = m_Owner.Dir;
            hurtData.AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * dir, m_SkillEffect.AddTargetForce.y);
            hurtData.AttackerPos = m_Owner.Pos;
            hurtData.CanBeDefense = m_SkillEffect.CanBeDefense;
            hurtData.IsSwoon = m_SkillEffect.IsSmoon;
            hurtData.AttackerID = m_Owner.ID;
            hurtData.AttackValue = 1;
            hurtData.HurtSound = m_SkillData.HurtSound;
            hurtData.HurtAnim = string.Empty;
            hurtData.IsGroundHurt = m_SkillEffect.IsOnGroundHurt;
            hit.OnHurtMsg(hurtData);

            return true;
        }

        return false;
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

    }
}