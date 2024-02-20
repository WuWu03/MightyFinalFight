using GameFrameWork.GameEntity;
using UnityEngine;

public abstract class BaseCtrl
{
    public BaseRole owner
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

    public void Start()
    {
        if(m_IsRunning)
        {
            return;
        }

        m_IsRunning = true;
        OnStart();
    }

    public void Stop()
    {
        m_IsRunning = false;
    }

    public bool IsRunning()
    {
        return m_IsRunning;
    }

    public void Update()
    {
        if (m_Owner == null || !m_IsRunning)
        {
            return;
        }
        OnUpdate();
    }

    public void LateUpdate()
    {
        if (m_Owner == null || !m_IsRunning)
        {
            return;
        }
        OnLateUpdate();
    }

    public void Release()
    {
        OnRelease();
        m_Owner = null;
        m_IsRunning = false;
    }

    protected virtual void OnInit() { }

    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnRelease() { }

    protected BaseRole m_Owner = null;
    private bool m_IsRunning = false;
}