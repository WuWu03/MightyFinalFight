/*******************************************************/
/**2021-7-23 10:03**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using UnityEngine;
using static LoadPanelMgr;

public class LoadView : UIBaseView<LoadComponent, LoadSettings>
{

    protected override void OnOpen(object arg)
    {

    }

    protected override void OnShow(object arg)
    {

    }
    
    protected override void OnUpdate()
    {
        if (!m_IsFading && LoadPanelMgr.instance.fadeInfoCount > 0)
        {
            Fade(LoadPanelMgr.instance.GetFadeInfo());
        }
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {
        m_IsFading = false;
    }

    protected override void OnDestroy()
    {

    }

    private void Fade(FadeArgs fadeInfo)
    {
        if (fadeInfo == null)
        {
            return;
        }

        m_IsFading = true;
        m_OnComplete = fadeInfo.onComplete;
        component.imgShade.DOKill();
        component.imgShade.color = new Color(0, 0, 0, fadeInfo.from);
        component.imgShade.DOFade(fadeInfo.to, fadeInfo.duration).SetDelay(fadeInfo.delay).OnComplete(OnFadeComplete);
        fadeInfo.Release();
    }

    private void OnFadeComplete()
    {
        m_OnComplete?.Invoke();
        m_OnComplete = null;
        m_IsFading = false;
    }

    private GameFrameWorkAction m_OnComplete = null;
    private bool m_IsFading = false;
}