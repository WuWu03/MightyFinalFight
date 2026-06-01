/*
 * @Desc: Load 模块 LoadPresenter 界面视图
 * @Date: 2021-07-23 10:03:48
 * @Author: WuWu
 */

using DG.Tweening;
using WuWuFramework.UI;
using UnityEngine;
using static LoadMgr;

public class LoadPresenter : UIBaseViewPresenter<LoadView>
{
    protected override void OnOpen(object arg)
    {

    }

    protected override void OnShow(object arg)
    {

    }

    protected override void OnUpdate()
    {
        if (!m_IsFading)
        {
            if (m_CurrFadeInfo != null)
            {
                m_CurrFadeInfo.onComplete?.Invoke();
                m_CurrFadeInfo.Release();
                m_CurrFadeInfo = null;
            }

            if (LoadMgr.instance.fadeInfoCount > 0)
            {
                Fade(LoadMgr.instance.GetFadeInfo());
            }
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
        m_CurrFadeInfo = fadeInfo;
        view.imgShade.DOKill();
        view.imgShade.color = new Color(0, 0, 0, fadeInfo.from);
        view.imgShade.DOFade(fadeInfo.to, fadeInfo.duration).SetDelay(fadeInfo.delay).OnComplete(OnFadeComplete);
    }

    private void OnFadeComplete()
    {
        m_IsFading = false;
    }

    private FadeArgs m_CurrFadeInfo = null;
    private bool m_IsFading = false;
}