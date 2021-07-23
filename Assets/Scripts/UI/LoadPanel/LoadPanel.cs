/*******************************************************/
/**2021-7-23 10:03**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork;
using System;

public class LoadPanel : BasePanel
{
	public override string PanelName { get { return "LoadPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new LoadPanelComponent(UIRefRoot);
	}

    protected override void OnOpen()
	{
		if (m_IsDoFade)
		{
			m_IsDoFade = false;
			m_Component.ImgShade.DOFade(m_EndValue, m_Duration).SetDelay(m_Delay).OnComplete(OnComplete);
		}
	}

	public void DOFade(float endValue, float duration, float delay, GameFrameWorkAction onComplete)
	{
		m_OnComplete = onComplete;
		m_Duration = duration;
		m_EndValue = endValue;
		m_Delay = delay;
		m_IsDoFade = true;

		if (m_Component != null)
		{
			m_IsDoFade = false;
			m_Component.ImgShade.DOFade(endValue, duration).SetDelay(delay).OnComplete(OnComplete);
		}
	}

	private void OnComplete()
	{
		m_OnComplete?.Invoke();
		m_OnComplete = null;
	}

	protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
		m_IsDoFade = false;
		m_Duration = 0;
		m_EndValue = 0;
		m_Delay = 0;
		m_OnComplete = null;
	}

	protected override void OnDestroy()
	{

	}

	private bool m_IsDoFade = false;
	private float m_Duration = 0;
	private float m_EndValue = 0;
	private float m_Delay = 0;
	private GameFrameWorkAction m_OnComplete = null;
	private LoadPanelComponent m_Component = null;
}