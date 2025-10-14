using GameFrameWork;
using GameFrameWork.Event;

public class CallbackClip : BaseClip
{
    public static CallbackClip Create(GameFrameWorkAction gameFrameWorkAction)
    {
        CallbackClip clip = ReferencePool.Acquire<CallbackClip>();
        clip.m_GameFrameWorkAction = gameFrameWorkAction;
        return clip;
    }

    protected override void OnClear()
    {
        m_GameFrameWorkAction = null;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        m_GameFrameWorkAction?.Invoke();
        Complete();
    }

    protected override void OnResume()
    {

    }

    private GameFrameWorkAction m_GameFrameWorkAction = null;
}
