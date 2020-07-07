using UnityEngine;
using FrameWork.GameEntity;

public class BaseSceneObject : BaseObject
{
    public bool IsInGround
    {
        get
        {
            return transform.localPosition.y <= m_Pos.y;
        }
    }

    public int Health
    {
        get { return m_Health; }
        set { m_Health = value; }
    }

    public int MaxHealth
    {
        get { return m_Health; }
        set { m_Health = value; }
    }

    public virtual void AddHealth(int value)
    {
        m_Health += value;
    }

    public virtual void AddMaxHealth(int value)
    {
        m_MaxHealth += value;
    }

    public  virtual void SubHealth(int value)
    {
        m_Health -= value;
        if (m_Health < 0) m_Health = 0;
    }

    public virtual void SubMaxHealth(int value)
    {
        m_MaxHealth -= value;
        if (m_MaxHealth < 0) m_MaxHealth = 0;
    }

    public virtual void InitData(BaseSceneObjectData data)
    {
        m_Health = data.Health;
        m_MaxHealth = data.MaxHealth;
    }

    protected int m_Health = 0;
    protected int m_MaxHealth = 0;
}
