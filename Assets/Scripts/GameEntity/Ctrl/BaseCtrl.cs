using FrameWork.GameEntity;
using UnityEngine;

public abstract class BaseCtrl
{
    public BaseObject Owner
    {
        get
        {
            return m_Owner;
        }
    }

    public virtual void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
    }

    public void Update()
    {
        if (m_Owner == null) return;
        OnUpdate();
    }

    public virtual void Release()
    {
        m_Owner = null;
    }

    protected virtual void OnUpdate() { }
    protected BaseRole m_Owner = null;
}