using GameFrameWork;
using GameFrameWork.Audio;

public class PauseBgmStory : BaseStory
{
    public static PauseBgmStory Create(float endValue, float delay, float duration)
    {
        PauseBgmStory pauseBgmStory = ReferencePool.Acquire<PauseBgmStory>();
        return pauseBgmStory;
    }

    public override bool IsStoryComplete()
    {
        return isPlaying;
    }

    protected override void OnClear()
    {

    }

    protected override void OnPauseStory()
    {

    }

    protected override void OnPlayStory()
    {
        AudioMgr.instance.PauseBgm();
    }

    protected override void OnResumeStory()
    {

    }

}
