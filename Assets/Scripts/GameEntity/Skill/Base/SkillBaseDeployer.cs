using System.Collections.Generic;
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
        m_SkillSelectors = SkillFactory.CreateSelector(m_SkillData, owner);
        m_SkillEffects = SkillFactory.CreateEffects(m_SkillData, owner);
        m_ListGroundEffect = new List<int>();
    }

    public void AddEvent()
    {
        m_HasAddForce = false;
        m_HasAddGroundForce = false;

        for (int i = 0; i < m_SkillEffects.Length; i++)
        {
            if (m_SkillEffects[m_CurrEffectIndex] == null)
            {
                continue;
            }

            if (m_SkillData.SkillEffects[i].IsOnGroundEffect)
            {
                m_Owner.OnGroundEvent.AddListener(OnGround);
                break;
            }
        }
    }

    public void RemoveEvent()
    {
        m_Owner.OnGroundEvent.RemoveListener(OnGround);
        OnRemoveEvent();
    }

    public virtual void DeploySkill()
    {
        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Enternal)
        {
            m_EnternalTriggerTimer = Time.time;
        }

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

        if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Enternal)
        {
            if (ret)
            {
                ResetEffect();
            }

            ret = ret && Time.time - m_EnternalTriggerTimer >= m_SkillData.EnternalTiggerTime;
        }
        else if (m_SkillData.TriggerType == SkillConfigData.SkillTriggerType.Animtion)
        {
            ret = ret && m_Owner.IsPlayComplete();

            if (ret)
            {
                OnAnimationEffectComplete();
                ResetEffect();
            }
        }
        else if(ret)
        {
            ResetEffect();
        }


        return ret;
    }

    public virtual void OnExit()
    {
        for (int i = 0; i < m_SkillEffects.Length; i++)
            m_SkillEffects[i].Exit();
        for (int i = 0; i < m_SkillSelectors.Length; i++)
            if (m_SkillSelectors[i] != null)
                m_SkillSelectors[i].Exit();

        m_CurrEffectIndex = 0;
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
                m_SkillEffects[i].Update(m_SkillSelectors[i]);
        }
    }

    private void AnimationEffect()
    {
        if (!m_SkillData.SkillEffects[m_CurrEffectIndex].IsOnGroundEffect)
        {
            CheckAddSelfForce(m_SkillData.SkillEffects[m_CurrEffectIndex].AddSelfForce);
            m_SkillEffects[m_CurrEffectIndex].Effect(m_SkillSelectors[m_CurrEffectIndex]);
        }
        else
        {
            if (m_Owner.IsFloat && !m_ListGroundEffect.Contains(m_CurrEffectIndex))
                m_ListGroundEffect.Add(m_CurrEffectIndex);
        }

        m_CurrEffectIndex++;

        if (m_CurrEffectIndex >= m_SkillEffects.Length)
        {
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
                {
                    CheckAddSelfForce(m_SkillData.SkillEffects[i].AddSelfForce);
                }

                m_SkillEffects[i].Effect(m_SkillSelectors[i]);
            }
            else
            {
                if (!m_Owner.IsInGround && !m_ListGroundEffect.Contains(i))
                    m_ListGroundEffect.Add(i);
            }
        }

        m_HasAddForce = true;
    }

    private void CheckAddSelfForce(Vector2 addSelfForce, bool isGround = false)
    {
        if (addSelfForce.x != 0 || addSelfForce.y != 0)
        {
            if (isGround)
            {
                m_Owner.SetVelocity(Vector2.zero);
            }

            m_Owner.AddForce(addSelfForce.x * m_Owner.Dir, addSelfForce.y, isGround);
        }
    }

    private void OnGround()
    {
        for (int i = 0; i < m_ListGroundEffect.Count; i++)
        {
            int index = m_ListGroundEffect[i];

            if (!m_HasAddGroundForce)
            {
                CheckAddSelfForce(m_SkillData.SkillEffects[index].AddSelfForce, true);
            }

            m_SkillEffects[index].Effect(m_SkillSelectors[index]);
        }

        m_HasAddGroundForce = true;
        m_ListGroundEffect.Clear();
    }

    private void ResetEffect()
    {
        for (int i = 0; i < m_SkillEffects.Length; i++)
            m_SkillEffects[i].Reset();
        for (int i = 0; i < m_SkillSelectors.Length; i++)
            m_SkillSelectors[i].Reset();
    }

    protected virtual void OnAnimationEffectComplete() { }

    protected virtual void OnRemoveEvent() { }

    protected BaseRole m_Owner = null;
    protected SkillConfigData m_SkillData = null;

    private List<int> m_ListGroundEffect = null;
    private int m_SkillId = 0;
    private int m_CurrEffectIndex = 0;
    private float m_EnternalTriggerTimer = 0f;
    private bool m_HasAddGroundForce = false;
    private bool m_HasAddForce = false;
    private ISkillSelector[] m_SkillSelectors = null;
    private ISkillEffect[] m_SkillEffects = null;
}