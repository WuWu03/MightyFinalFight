using GameFrameWork;

public class PauseBgmClip : BaseClip
{
    public static PauseBgmClip Create()
    {
        PauseBgmClip pauseBgmStory = ReferencePool.Acquire<PauseBgmClip>();
        return pauseBgmStory;
    }

    protected override void OnClear()
    {

    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        GameEntry.soundMgr.PauseBgm();
        Complete();
    }

    protected override void OnResume()
    {

    }
}
