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
		m_Component.ImgShade.color = new Color(0, 0, 0, 0);

		if (m_IsDoFade)
		{
			if (!m_IsAuto)
			{
				m_Component.ImgShade.DOFade(m_EndValue, m_Duration).SetDelay(m_Delay).OnComplete(OnComplete);
			}
			else
			{
				m_Component.ImgShade.DOFade(1, m_Duration).SetDelay(m_Delay).OnComplete(OnAutoFadeComplete);
			}

			m_IsDoFade = false;
		}
	}

	public void DOFade(float endValue, float duration, float delay, GameFrameWorkAction onComplete)
	{
		m_OnComplete = onComplete;
		m_Duration = duration;
		m_EndValue = endValue;
		m_Delay = delay;
		m_IsDoFade = true;
		m_IsAuto = false;

		if (m_Component != null)
		{
			m_IsDoFade = false;
			m_Component.ImgShade.DOFade(endValue, duration).SetDelay(delay).OnComplete(OnComplete);
		}
	}

	public void DOFadeAuto(float duration,float delay, GameFrameWorkAction onComplete)
    {
		m_OnComplete = onComplete;
		m_Duration = duration;
		m_EndValue = 0;
		m_Delay = delay;
		m_IsDoFade = true;
		m_IsAuto = true;

		if (m_Component != null)
		{
			m_IsDoFade = false;
			m_Component.ImgShade.DOFade(1, duration).SetDelay(delay).OnComplete(OnAutoFadeComplete);
		}
	}

	private void OnAutoFadeComplete()
    {
		m_Component.ImgShade.DOFade(0, m_Duration).OnComplete(OnComplete);
	}

	private void OnComplete()
	{
		m_IsAuto = false;
		m_IsDoFade = false;
		m_OnComplete?.Invoke();
		m_OnComplete = null;
	}

	protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
		m_IsDoFade = false;
		m_IsAuto = false;
		m_Duration = 0;
		m_EndValue = 0;
		m_Delay = 0;
		m_OnComplete = null;
		m_Component.ImgShade.color = new Color(0, 0, 0, 0);
	}

	protected override void OnDestroy()
	{

	}

	private bool m_IsDoFade = false;
	private bool m_IsAuto = false;
	private float m_Duration = 0;
	private float m_EndValue = 0;
	private float m_Delay = 0;
	private GameFrameWorkAction m_OnComplete = null;
	private LoadPanelComponent m_Component = null;
}