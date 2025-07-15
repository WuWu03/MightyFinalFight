/*******************************************************/
/**2021-7-23 10:03**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using UnityEngine;
using static LoadPanelMgr;

public class LoadPanel : BasePanel<LoadPanelComponent>
{

    protected override void OnInit( object[] param)
    {

    }

	protected override void OnOpen()
	{

	}

    protected override void OnUpdate()
    {
        if (!m_IsFading && LoadPanelMgr.instance.fadeInfoCount > 0)
        {
            Fade(LoadPanelMgr.instance.GetFadeInfo());
        }
    }

	protected override void OnClose()
	{
        m_IsFading = false;
    }

	protected override void OnDestroy()
	{

	}

    private void Fade(FadeInfo fadeInfo)
    {
        if (fadeInfo == null)
        {
            return;
        }

        m_IsFading = true;
        m_Component.imgShade.DOKill();
        m_Component.imgShade.color = new Color(0, 0, 0, fadeInfo.from);
        m_Component.imgShade.DOFade(fadeInfo.to, fadeInfo.duration).SetDelay(fadeInfo.delay).OnComplete(() =>
        {
            fadeInfo.onComplete?.Invoke();
            ReferencePool.ReleaseReference(fadeInfo);
            m_IsFading = false;
        });
    }

    private bool m_IsFading = false;
}