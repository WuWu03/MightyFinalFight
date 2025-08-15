using GameFrameWork;
using GameFrameWork.Audio;

public class FadeBgmClip : BaseClip
{
    public static FadeBgmClip Create(float endValue, float delay, float duration)
    {
        FadeBgmClip fadeBgmStory = ReferencePool.Acquire<FadeBgmClip>();
        fadeBgmStory.m_EndValue = endValue;
        fadeBgmStory.m_Delay = delay;
        fadeBgmStory.m_Duration = duration;
        return fadeBgmStory;
    }

    protected override void OnClear()
    {
        m_EndValue = 0f;
        m_Delay = 0f;
        m_Duration = 0f;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        AudioMgr.instance.onBgmFadeCompleteEvent += Complete;
        AudioMgr.instance.FadeBgm(m_EndValue, m_Delay, m_Duration);
    }


    protected override void OnResume()
    {

    }

    private float m_EndValue;
    private float m_Delay;
    private float m_Duration;
}
