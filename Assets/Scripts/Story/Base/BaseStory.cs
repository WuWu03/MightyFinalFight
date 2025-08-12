using GameFrameWork;

public abstract class BaseStory : IStory, IReference
{
    public bool isWaitComplete { get; protected set; }
    public bool isPlaying
    {
        get
        {
            return m_IsRun && m_IsPlaying;
        }
    }

    public void PlayStory()
    {
        OnPauseStory();
        m_IsPlaying = true;
        m_IsRun = true;
    }
    public void PauseStory()
    {
        if (!m_IsRun || !m_IsPlaying)
        {
            return;
        }

        OnPauseStory();
    }
    public void ResumeStory()
    {
        if (!m_IsRun || m_IsPlaying)
        {
            return;
        }

        OnResumeStory();
    }

    public abstract bool IsStoryComplete();
    protected abstract void OnPlayStory();
    protected abstract void OnPauseStory();
    protected abstract void OnResumeStory();
    protected abstract void OnClear();

    public void Release()
    {
        ReferencePool.Release(this);
    }

    public void Clear()
    {
        m_IsPlaying = false;
        m_IsRun = false;
        OnClear();
    }

    private bool m_IsPlaying = false;
    private bool m_IsRun = false;
}
