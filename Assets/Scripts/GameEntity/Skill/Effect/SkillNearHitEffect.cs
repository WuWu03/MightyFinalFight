using FrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNearHitEffect : ISkillEffect
{
    public SkillNearHitEffect()
    {
        m_HurtData = new HurtData();
    }

    public bool IsCompleted
    {
        get
        {
            if (m_SkillData != null && m_SkillData.DeployeType == SkillData.SkillDeployeType.Animtion)
            {
                m_Complete = m_Owner.IsPlayComplete();
            }

            return m_Complete;
        }
    }

    public int Index
    {
        get;
        set;
    }

    public void Effect(BaseRole owner, SkillData skillData, ISkillSelector skillSelector)
    {
        m_Owner = owner;
        m_SkillData = skillData;
        m_Complete = false;

        bool hurtTarget = false;
        List<ICanBeHit> targets = m_Owner.OnHitStart();
        
        if(targets == null)
        {
            targets = skillSelector.GetTargets(owner, skillData);
        }
        
        for (int i = 0; i < targets.Count; i++)
        {
            if(Hit(targets[i],owner,skillData))
            {
                hurtTarget = true;
            }
        }

        owner.OnHitEnd(skillData, hurtTarget);
        m_Complete = true;
    }

    private bool Hit(ICanBeHit hit,BaseRole owner,SkillData skillData)
    {
        if (hit != null && hit.CanBeHit)
        {
            float dir = hit.HurtPos.x - owner.Pos.x >= 0 ? 1 : -1;
            if(skillData.SkillEffects[Index].ForceType == SkillData.SkillAddForceType.SelfDir)
            {
                dir = owner.Dir;
            }

            m_HurtData.AttackerDir = owner.Dir;
            m_HurtData.AttackForce = new Vector2(skillData.SkillEffects[Index].AddTargetForce.x * dir, skillData.SkillEffects[Index].AddTargetForce.y);
            m_HurtData.IsSwoon = skillData.SkillEffects[Index].IsSmoon;
            m_HurtData.AttackerID = owner.ID;
            m_HurtData.AttackValue = 1;
            m_HurtData.HurtSound = string.Empty;

            hit.OnHurtMsg(m_HurtData);

            if (skillData.SkillEffects[Index].IsShakeCamera)
            {
                CameraMgr.Ins.Shake();
            }

            return true;
        }

        return false;
    }

    public void Reset()
    {
        m_Complete = false;
        m_Owner = null;
        m_SkillData = null;
    }

    private HurtData m_HurtData = null;
    private BaseRole m_Owner = null;
    private SkillData m_SkillData = null;
    private bool m_Complete = false;
}