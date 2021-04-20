using GameFrameWork.GameEntity;
using UnityEngine;

public class BaseEffect : BaseSceneObject
{
    public float PlayTime
    {
        get;
        set;
    }

    public VoidNotPar PlayEndCallback
    {
        get;
        set;
    }

    public bool IsAutoRelease
    {
        get;
        set;
    }

    public void Play()
    {
        m_IsPlaying = true;
        if (m_UAC != null)
        {
            m_UAC.animation.Play();
        }
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
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
        if (m_IsPlaying && PlayTime > 0 && m_ResComplete)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer >= PlayTime)
            {
                m_IsPlaying = false;
                m_Timer = 0;
                PlayEndCallback?.Invoke();
                if (IsAutoRelease) Release();
            }
        }
    }

    private bool m_IsPlaying = false;
    private float m_Timer = 0;
    private DragonBones.UnityArmatureComponent m_UAC = null;
}