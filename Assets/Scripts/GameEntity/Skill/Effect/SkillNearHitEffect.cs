using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector skillSelector)
    {
        m_Targets = m_Owner.OnHitStart();

        if (m_Targets == null || m_Targets.Count < 1)
        {
            m_Targets = skillSelector.GetTargets();
        }

        if(m_Targets == null || m_Targets.Count < 1)
        {
            Complete();
            return;
        }

        m_IsPause = m_SkillData.Id == 1003006;

        if (!m_IsPause)
        {
            for (int i = 0; i < m_Targets.Count; i++)
            {
                if (SkillUtil.SkillHit(m_Targets[i], m_Owner, m_SkillData, m_SkillEffect) && !m_IsHurtTarget)
                {
                    m_IsHurtTarget = true;
                }
            }

            if (m_IsHurtTarget)
            {
                if (m_SkillEffect.IsShakeCamera)
                {
                    CameraMgr.instance.Shake();
                }
            }

            m_Owner.OnHitEnd(m_SkillData, m_IsHurtTarget);
            Complete();
        }
    }

    protected override void OnComplete()
    {
        base.OnComplete();
        m_IsHurtTarget = false;
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_OffsetTime = 0f;
        m_PauseIndex = 0;
        m_PauseTimer = -1f;
        m_IsHurtTarget = false;
        m_IsPause = false;
        m_Targets = null;
    }

    protected override void OnUpdate(ISkillSelector selector)
    {
        base.OnUpdate(selector);

        if (!m_IsPause)
        {
            return;
        }

        if (m_PauseIndex <= m_Targets.Count)
        {
            if (m_PauseTimer < 0f || Time.unscaledTime - m_PauseTimer > m_OffsetTime)
            {
                if (Time.timeScale > 0f && m_PauseTimer > 0f)
                {
                    Time.timeScale = 0f;
                    m_PauseTimer = Time.unscaledTime;
                    m_OffsetTime = 0.1f;
                    return;
                }
                
                if(Time.timeScale < 1f || m_PauseTimer < 0f)
                {
                    if(m_PauseIndex < m_Targets.Count)
                    {
                        HurtStateData hurtStateData = SkillUtil.GetHurtData(m_Targets[m_PauseIndex], m_Owner, m_SkillData, m_SkillEffect, true);

                        if (hurtStateData != null)
                        {
                            if (!m_IsHurtTarget)
                            {
                                m_IsHurtTarget = !m_Targets[m_PauseIndex].IsHurtWillDie(hurtStateData.attackValue);
                            }

                            m_Targets[m_PauseIndex].OnHurtMsg(hurtStateData);
                            m_OffsetTime = 0.05f;
                            Time.timeScale = 1f;
                            m_PauseTimer = Time.unscaledTime;
                        }
                    }
         
                    m_PauseIndex++;
                }
            }
        }
        else
        {
            Time.timeScale = 1f;

            if (m_IsHurtTarget)
            {
                if (m_SkillEffect.IsShakeCamera)
                {
                    CameraMgr.instance.Shake();
                }
            }

            m_Owner.OnHitEnd(m_SkillData, m_IsHurtTarget);
            Complete();
        }
    }

    private float m_OffsetTime = 0f;
    private int m_PauseIndex = 0;
    private float m_PauseTimer = -1f;
    private bool m_IsHurtTarget = false;
    private bool m_IsPause = false;
    private List<ICanBeHit> m_Targets = null;
}