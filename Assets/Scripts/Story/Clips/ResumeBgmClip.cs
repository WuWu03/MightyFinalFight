using GameFrameWork;
using GameFrameWork.Audio;

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
        AudioMgr.instance.ResumeBgm();
        Complete();
    }

    protected override void OnResume()
    {

    }

}
