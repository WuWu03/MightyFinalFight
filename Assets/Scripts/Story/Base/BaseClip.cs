using GameFrameWork;

public abstract class BaseClip : IStory, IReference
{
    public bool isPlaying
    {
        get
        {
            return m_IsRun && m_IsPlaying;
        }
    }

    public void Play()
    {
        if (m_IsRun) 
        {
            return;
        }

        OnPlay();
        m_IsPlaying = true;
        m_IsRun = true;
    }

    public void Pause()
    {
        if (!m_IsRun || !m_IsPlaying)
        {
            return;
        }

        m_IsPlaying = false;
        OnPause();
    }

    public void Resume()
    {
        if (!m_IsRun || m_IsPlaying)
        {
            return;
        }

        m_IsPlaying = true;
        OnResume();
    }
    public void Release()
    {
        ReferencePool.Release(this);
    }

    public void Clear()
    {
        m_IsPlaying = false;
        m_IsRun = false;
        m_IsComplete = false;
        OnClear();
    }

    public virtual bool IsComplete()
    {
        return m_IsComplete;
    }

    protected abstract void OnPlay();
    protected abstract void OnPause();
    protected abstract void OnResume();
    protected abstract void OnClear();

    protected void Complete()
    {
        m_IsComplete = true;
    }

    private bool m_IsComplete = false;
    private bool m_IsPlaying = false;
    private bool m_IsRun = false;
}
