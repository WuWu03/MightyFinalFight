using WuWuFramework;

public class ResumeBgmClip : BaseClip
{
    public static ResumeBgmClip Create()
    {
        return ReferencePool.Acquire<ResumeBgmClip>();
    }

    protected override void OnClear()
    {

    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        GameEntry.soundMgr.ResumeBgm();
        Complete();
    }

    protected override void OnResume()
    {

    }
}
