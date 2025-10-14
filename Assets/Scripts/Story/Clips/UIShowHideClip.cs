using System;
using GameFrameWork;
using GameFrameWork.UI;

public class UIShowHideClip : BaseClip
{
    public static UIShowHideClip Create(Type uiType, bool isActive)
    {
        UIShowHideClip clip = ReferencePool.Acquire<UIShowHideClip>();
        clip.m_UIType = uiType;
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
            UIMgr.instance.Get(m_UIType).Show();
        }
        else
        {
            UIMgr.instance.Get(m_UIType).Hide();
        }

        Complete();
    }

    protected override void OnResume()
    {

    }

    private Type m_UIType = null;
    private bool m_IsActive = false;
}
