using DragonBones;
using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Sound;
using UnityEngine;

public class BaseEffect : BaseSceneObject
{
    public float PlayTime
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

    public float Speed
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

    public GameFrameWorkAction PlayEndCallback
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

    public bool IsAutoRelease
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

    public bool IsPlaying
    {
        get
        {
            return m_IsPlaying;
        }
    }

    public virtual void Play()
    {
        m_IsPlaying = true;

        if (m_IsResComplete)
        {
            m_Timer = Time.time;
            m_ResGO.SetActive(true);
        }
    }

    public override void Release()
    {
        base.Release();
        m_PlayTime = 0;
        m_Timer = -1;
        m_IsAutoRelease = false;
        m_IsPlaying = false;
        m_PlayEndCallback = null;
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        if (m_IsPlaying)
        {
            m_Timer = Time.time;
            m_ResGO.SetActive(true);
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (!m_IsPlaying || m_PlayTime <= 0 || m_Timer < 0 || !m_IsResComplete)
        {
            return;
        }

        if (Time.time - m_Timer < m_PlayTime)
        {
            return;
        }

        m_IsPlaying = false;
        m_Timer = -1;
        PlayEndCallback?.Invoke();

        if (IsAutoRelease)
        {
            EffectMgr.Ins.PutEffect(this);
        }
    }

    protected float m_Speed = 1f;
    protected bool m_IsPlaying = false;

    private float m_PlayTime = 0;
    private float m_Timer = -1;
    private bool m_IsAutoRelease = false;
    private GameFrameWorkAction m_PlayEndCallback = null;
}