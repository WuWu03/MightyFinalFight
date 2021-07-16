using GameFrameWork;
using GameFrameWork.GameEntity;
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

    public void Play()
    {
        m_IsPlaying = true;
        m_Timer = Time.time;

        if (m_UAC != null)
        {
            m_UAC.animation.Play();
        }
    }

    public override void Release()
    {
        base.Release();
        m_UAC.animation.Stop();
        m_PlayTime = 0;
        m_Timer = -1;
        m_IsAutoRelease = false;
        m_IsPlaying = false;
        m_PlayEndCallback = null;
        m_UAC = null;
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_UAC = go.GetComponent<DragonBones.UnityArmatureComponent>();
        m_UAC.animation.Stop();

        if (m_IsPlaying)
        {
            m_UAC.animation.Play();
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

    private float m_PlayTime = 0;
    private float m_Timer = -1;
    private bool m_IsAutoRelease = false;
    private bool m_IsPlaying = false;
    private GameFrameWorkAction m_PlayEndCallback = null;
    private DragonBones.UnityArmatureComponent m_UAC = null;
}