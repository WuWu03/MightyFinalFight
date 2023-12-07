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
	public override string panelName { get { return "LoadPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.ThirdLevel; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new LoadPanelComponent(m_UIRefRoot);
	}

	protected override void OnOpen()
	{
		if (m_IsDoFade)
		{
			m_IsDoFade = false;
			m_Component.imgShade.DOKill();
			m_Component.imgShade.color = new Color(0, 0, 0, m_From);

			if (!m_IsAuto)
			{
				m_Component.imgShade.DOFade(m_To, m_Duration).SetDelay(m_Delay).OnComplete(OnComplete);
			}
			else
			{
				m_Component.imgShade.DOFade(m_To, m_Duration).SetDelay(m_Delay).OnComplete(OnAutoFadeComplete);
			}
		}
	}

	public void DOFade(float from, float to, float duration, float delay, GameFrameWorkAction onComplete)
	{
		m_OnComplete = onComplete;
		m_Duration = duration;
		m_From = from;
		m_To = to;
		m_Delay = delay;
		m_IsDoFade = true;
		m_IsAuto = false;

		if (m_Component != null)
		{
			m_IsDoFade = false;
			m_Component.imgShade.DOKill();
			m_Component.imgShade.color = new Color(0, 0, 0, m_From);
			m_Component.imgShade.DOFade(m_To, duration).SetDelay(delay).OnComplete(OnComplete);
		}
	}

	public void DOFadeAuto(float duration, float delay, GameFrameWorkAction onComplete)
	{
		m_OnComplete = onComplete;
		m_Duration = duration;
		m_From = 0;
		m_To = 1;
		m_Delay = delay;
		m_IsDoFade = true;
		m_IsAuto = true;

		if (m_Component != null)
		{
			m_IsDoFade = false;
			m_Component.imgShade.DOKill();
			m_Component.imgShade.color = new Color(0, 0, 0, m_From);
			m_Component.imgShade.DOFade(m_To, duration).SetDelay(delay).OnComplete(OnAutoFadeComplete);
		}
	}

	private void OnAutoFadeComplete()
    {
		m_To = 0;
		m_Component.imgShade.DOFade(m_To, m_Duration).OnComplete(OnComplete);
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
		m_To = 0;
		m_Delay = 0;
		m_OnComplete = null;
	}

	protected override void OnDestroy()
	{

	}

	private bool m_IsDoFade = false;
	private bool m_IsAuto = false;
	private float m_Duration = 0;
	private float m_From = 0;
	private float m_To = 0;
	private float m_Delay = 0;
	private GameFrameWorkAction m_OnComplete = null;
	private LoadPanelComponent m_Component = null;
}