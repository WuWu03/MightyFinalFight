using GameFrameWork;
using GameFrameWork.Audio;

public class FadeBgmStory : BaseStory
{
    public static FadeBgmStory Create(float endValue, float delay, float duration)
    {
        FadeBgmStory fadeBgmStory = ReferencePool.Acquire<FadeBgmStory>();
        fadeBgmStory.m_EndValue = endValue;
        fadeBgmStory.m_Delay = delay;
        fadeBgmStory.m_Duration = duration;
        return fadeBgmStory;
    }

    public override bool IsStoryComplete()
    {
        return isPlaying && m_IsFadeEnd;
    }

    protected override void OnClear()
    {
        m_EndValue = 0f;
        m_Delay = 0f;
        m_Duration = 0f;
        m_IsFadeEnd = false;
    }

    protected override void OnPauseStory()
    {

    }

    protected override void OnPlayStory()
    {
        AudioMgr.instance.onBgmFadeCompleteEvent += OnBgmFadeCompleteEvent;
        AudioMgr.instance.FadeBgm(m_EndValue, m_Delay, m_Duration);
    }

    private void OnBgmFadeCompleteEvent()
    {
        m_IsFadeEnd = true;
    }

    protected override void OnResumeStory()
    {

    }

    private float m_EndValue;
    private float m_Delay;
    private float m_Duration;
    private bool m_IsFadeEnd = false;
}
