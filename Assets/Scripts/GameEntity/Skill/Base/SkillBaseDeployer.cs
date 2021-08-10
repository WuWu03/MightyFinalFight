
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class SkillBaseDeployer
{
    public int SkillId
    {
        get
        {
            return m_SkillId;
        }
    }

    public SkillConfigData SkillData
    {
        get 
        { 
            return m_SkillData; 
        }
    }

    public SkillBaseDeployer(int skillId, BaseRole owner)
    {
        m_SkillId = skillId;
        m_Owner = owner;
        m_SkillData = StaticConfig.SkillConfig.GetData(skillId);
        m_SkillSelector = SkillFactory.CreateSelector(m_SkillData, owner);
        m_SkillEffects = SkillFactory.CreateEffects(m_SkillData, owner);
        m_ListGroundEffect = new List<int>();
    }

    public void AddEvent()
    {
        m_Owner.OnGroundEvent.AddListener(OnGround);
    }

    public virtual void DeploySkill()
    {
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Enternal)
            m_EnternalTriggerTimer = Time.time;

        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
        {
            AnimationEffect();
        }
        else
        {
            JustEffect();
        }
    }

    public virtual bool IsAllComplete()
    {
        bool ret = true;

        if (m_ListGroundEffect.Count > 0)
        {
            ret = false;
        }

        if (ret)
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
            {
                if (!m_SkillEffects[i].IsCompleted)
                {
                    ret = false;
                    break;
                }
            }
        }

        if (ret)
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
                m_SkillEffects[i].Reset();
            for (int i = 0; i < m_SkillSelector.Length; i++)
                m_SkillSelector[i].Reset();
        }

        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Enternal)
        {
            ret = ret && Time.time - m_EnternalTriggerTimer >= m_SkillData.EnternalTiggerTime;
        }

        return ret;
    }

    private void AnimationEffect()
    {
        if (!m_SkillData.SkillEffects[m_CurrEffectIndex].IsOnGroundEffect)
        {
            CheckAddSelfForce(m_SkillData.SkillEffects[m_CurrEffectIndex].AddSelfForce);
            m_SkillEffects[m_CurrEffectIndex].Effect(m_SkillSelector[m_CurrEffectIndex]);
        }
        else
        {
            if (m_Owner.IsFloat && !m_ListGroundEffect.Contains(m_CurrEffectIndex))
                m_ListGroundEffect.Add(m_CurrEffectIndex);
        }

        m_CurrEffectIndex++;

        if (m_CurrEffectIndex >= m_SkillEffects.Length)
        {
            OnEffectComplete();
            m_CurrEffectIndex = 0;
        }
    }

    private void JustEffect()
    {
        m_CurrEffectIndex = 0;
        for (int i = 0; i < m_SkillEffects.Length; i++)
        {
            if (m_SkillEffects[m_CurrEffectIndex] == null) continue;

            if (!m_SkillData.SkillEffects[i].IsOnGroundEffect)
            {
                if (!m_HasAddForce)
                    CheckAddSelfForce(m_SkillData.SkillEffects[i].AddSelfForce);
                m_SkillEffects[i].Effect(m_SkillSelector[i]);
            }
            else
            {
                if (m_Owner.IsFloat && !m_ListGroundEffect.Contains(i))
                    m_ListGroundEffect.Add(i);
            }
        }

        m_HasAddForce = true;
    }

    private void CheckAddSelfForce(Vector2 addSelfForce)
    {
        if (addSelfForce.x != 0 || addSelfForce.y != 0)
        {
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            m_Owner.Rigidbody.AddForce(new Vector2(addSelfForce.x * m_Owner.Dir, addSelfForce.y));
        }
    }

    private void OnGround()
    {
        for (int i = 0; i < m_ListGroundEffect.Count; i++)
        {
            int index = m_ListGroundEffect[i];
            CheckAddSelfForce(m_SkillData.SkillEffects[index].AddSelfForce);
            m_SkillEffects[index].Effect(m_SkillSelector[index]);      
        }

        m_ListGroundEffect.Clear();
    }

    public virtual void OnExit()
    {
        for (int i = 0; i < m_SkillEffects.Length; i++)
            m_SkillEffects[i].Exit();
        for (int i = 0; i < m_SkillSelector.Length; i++)
            if (m_SkillSelector[i] != null)
                m_SkillSelector[i].Exit();

        m_CurrEffectIndex = 0;
        m_HasAddForce = false;
    }

    public virtual void Update() 
    {
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Enternal)
        {
            JustEffect();
        }

        for (int i = 0; i < m_SkillEffects.Length; i++)
        {
            if (!m_SkillEffects[i].IsCompleted)
                m_SkillEffects[i].Update(m_SkillSelector[i]);
        }
    }

    protected virtual void OnEffectComplete() { }
    protected BaseRole m_Owner = null;
    protected SkillConfigData m_SkillData = null;
    protected List<int> m_ListGroundEffect = null;
    private int m_SkillId = 0;
    private int m_CurrEffectIndex = 0;
    private float m_EnternalTriggerTimer = 0f;
    private bool m_HasAddForce = false;
    private ISkillSelector[] m_SkillSelector = null;
    private ISkillEffect[] m_SkillEffects = null;
}