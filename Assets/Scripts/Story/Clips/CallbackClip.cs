using WuWuFramework;
using WuWuFramework.Event;

public class CallbackClip : BaseClip
{
    public static CallbackClip Create(WuWuFrameworkAction callbackAction)
    {
        CallbackClip clip = ReferencePool.Acquire<CallbackClip>();
        clip.m_CallbackAction = callbackAction;
        return clip;
    }

    protected override void OnClear()
    {
        m_CallbackAction = null;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        m_CallbackAction?.Invoke();
        Complete();
    }

    protected override void OnResume()
    {

    }

    private WuWuFrameworkAction m_CallbackAction = null;
}
