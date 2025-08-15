using GameFrameWork;
using GameFrameWork.UI;

public class UIShowHideClip : BaseClip
{
    public static UIShowHideClip Create(string uiName, bool isActive)
    {
        UIShowHideClip clip = ReferencePool.Acquire<UIShowHideClip>();
        clip.m_UIName = uiName;
        clip.m_IsActive = isActive;
        return clip;
    }

    protected override void OnClear()
    {

    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        if (m_IsActive) 
        {
            UIMgr.instance.Get(m_UIName).Show();
        }
        else
        {
            UIMgr.instance.Get(m_UIName).Hide();
        }

        Complete();
    }

    protected override void OnResume()
    {

    }

    private string m_UIName = string.Empty;
    private bool m_IsActive = false;
}
