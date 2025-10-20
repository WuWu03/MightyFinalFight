using System;
using GameFrameWork;
using GameFrameWork.UI;

public class UIShowHideClip : BaseClip
{
    private Type m_UIType;
    private bool m_IsActive;
    
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
            GameFrameWorkMgr.GetModule<IUIMgr>().Get(m_UIType).Show();
        }
        else
        {
            GameFrameWorkMgr.GetModule<IUIMgr>().Get(m_UIType).Hide();
        }

        Complete();
    }

    protected override void OnResume()
    {

    }
}
