using GameFrameWork.GameEntity;
using UnityEngine;

public abstract class BaseCtrl
{
    public BaseRole Owner
    {
        get
        {
            return m_Owner;
        }
    }

    public void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
        OnInit();
    }

    public void Update()
    {
        if (m_Owner == null || !m_Owner.ResComplete) return;
        OnUpdate();
    }

    public void Release()
    {
        m_Owner = null;
        OnRelease();
    }

    protected virtual void OnInit() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnRelease() { }

    protected BaseRole m_Owner = null;
}