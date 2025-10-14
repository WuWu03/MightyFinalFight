using GameFrameWork;
using GameFrameWork.Event;
using UnityEngine;

public class BaseEffect : BaseSceneObject
{
    public float playTime
    {
        get
        {
            return m_PlayTime;
        }
        set
        {
            m_PlayTime = value;
        }
    }

    public float speed
    {
        get
        {
            return m_Speed;
        }
        set
        {
            m_Speed = value;
        }
    }

    public GameFrameWorkAction onPlayEndEvent
    {
        get
        {
            return m_PlayEndCallback;
        }
        set
        {
            m_PlayEndCallback = value;
        }
    }

    public bool isAutoRelease
    {
        get
        {
            return m_IsAutoRelease;
        }
        set
        {
            m_IsAutoRelease = value;
        }
    }

    public bool isPlaying
    {
        get
        {
            return m_IsPlaying;
        }
    }

    public virtual void Play()
    {
        m_IsPlaying = true;

        if (isAssetLoadComplete)
        {
            m_Timer = Time.time;
            asset.SetActiveSelf(true);
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_PlayTime = 0;
        m_Timer = -1;
        m_IsAutoRelease = false;
        m_IsPlaying = false;
        m_PlayEndCallback = null;
    }

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        if (m_IsPlaying)
        {
            m_Timer = Time.time;
            asset.SetActiveSelf(true);
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (!m_IsPlaying || m_PlayTime <= 0 || m_Timer < 0 || !isAssetLoadComplete)
        {
            return;
        }

        if (Time.time - m_Timer < m_PlayTime)
        {
            return;
        }

        m_IsPlaying = false;
        m_Timer = -1;
        onPlayEndEvent?.Invoke();

        if (isAutoRelease)
        {
            Release();
        }
    }

    protected float m_Speed = 1f;
    protected bool m_IsPlaying = false;

    private float m_PlayTime = 0;
    private float m_Timer = -1;
    private bool m_IsAutoRelease = false;
    private GameFrameWorkAction m_PlayEndCallback = null;
}