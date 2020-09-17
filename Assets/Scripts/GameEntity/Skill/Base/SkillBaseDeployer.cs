
using System.Diagnostics;

public abstract class SkillBaseDeployer
{
    public int SkillID { get; private set; }
    protected SkillData.SkillEffect CurrEffect
    {
        get
        {
            return m_SkillData.SkillEffects[m_CurrEffectIndex];
        }
    }

    public SkillData SkillData
    {
        get 
        { 
            return m_SkillData; 
        }
    }

    public SkillBaseDeployer(int skillID, BaseRole owner)
    {
        SkillID = skillID;
        m_Owner = owner;
        m_SkillData = StaticConfig.SkillConfig.GetData(skillID);
        m_SkillSelector = SkillFactory.CreateSelector(m_SkillData, owner);
        m_SkillEffects = SkillFactory.CreateEffects(m_SkillData, owner);
    }

    public virtual void DeploySkill()
    {
        if(SkillID == 2002)
        {
            UnityEngine.Debug.Log("技能出错");
        }
        if(m_SkillData.DeployeType == SkillData.SkillDeployeType.Animtion)
        {
            m_SkillEffects[m_CurrEffectIndex].Effect( m_SkillSelector[m_CurrEffectIndex]);
            m_CurrEffectIndex = m_CurrEffectIndex < m_SkillEffects.Length - 1 ? m_CurrEffectIndex + 1 : 0;
        }
        else
        {
            m_CurrEffectIndex = 0;
            for (int i = 0; i < m_SkillEffects.Length; i++)
            {
                m_SkillEffects[i].Effect(m_SkillSelector[i]);
            }
        }
    }

    public virtual bool IsAllComplete()
    {
        bool ret = true;

        for (int i = 0; i < m_SkillEffects.Length; i++)
        {
            if (!m_SkillEffects[i].IsCompleted)
            {
                ret = false;
                break;
            }
        }

        if (ret)
        {
            for (int i = 0; i < m_SkillEffects.Length; i++)
                m_SkillEffects[i].Reset();
            for (int i = 0; i < m_SkillSelector.Length; i++)
                m_SkillSelector[i].Reset();
        }

        return ret;
    }

    public virtual void OnExit()
    {
        for (int i = 0; i < m_SkillEffects.Length; i++)
            m_SkillEffects[i].Exit();
        for (int i = 0; i < m_SkillSelector.Length; i++)
            if (m_SkillSelector[i] != null)
                m_SkillSelector[i].Exit();

        m_CurrEffectIndex = 0;
    }

    public virtual void Update() 
    {
        for (int i = 0; i < m_SkillEffects.Length; i++)
            m_SkillEffects[i].Update();
    }

    protected BaseRole m_Owner = null;
    protected SkillData m_SkillData = null;
    private int m_CurrEffectIndex = 0;
    private ISkillSelector[] m_SkillSelector = null;
    private ISkillEffect[] m_SkillEffects = null;
}