using GameFrameWork.Audio;

public class ResumeBgmStory : BaseStory
{
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
