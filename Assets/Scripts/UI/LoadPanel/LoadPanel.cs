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
using System.Collections.Generic;
using Unity.VisualScripting;

public class LoadPanel : BasePanel
{
	public override string panelName { get { return "LoadPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer8; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Always; } }

	class FadeInfo
	{
		public float from;
		public float to;
		public float duration;
        public float delay;
		public GameFrameWorkAction onComplete;
    }

	protected override void OnInit(object[] param)
	{
		m_Component = new LoadPanelComponent(m_UIRefRoot);
    }

	protected override void OnOpen()
	{
		if (!m_IsDoing && m_QueueFade.Count > 0)
        {
            StartDoFade();
        }
	}

    protected override void OnUpdate()
    {

    }

    public void DOFade(float from, float to, float duration, float delay, GameFrameWorkAction onComplete)
	{
		if(m_QueueFade == null)
		{
			m_QueueFade = new Queue<FadeInfo>();
		}

		lock (m_QueueFade)
		{
            m_QueueFade.Enqueue(new FadeInfo()
            {
                from = from,
                to = to,
                duration = duration,
                delay = delay,
                onComplete = onComplete,
            });
        }

		if(!m_IsDoing)
		{
            StartDoFade();
        }
    }

	public void DOFadeAuto(float duration, float delay, GameFrameWorkAction onComplete)
	{
        if (m_QueueFade == null)
        {
            m_QueueFade = new Queue<FadeInfo>();
        }

		lock (m_QueueFade)
		{
			m_QueueFade.Enqueue(new FadeInfo()
			{
				from = 0,
				to = 1,
				duration = duration,
				delay = delay,
				onComplete = onComplete,
			});
		}

        if (!m_IsDoing)
        {
            StartDoFade();
        }
    }

	private void StartDoFade()
	{
		if (m_Component == null)
		{
			return;
		}

		if (m_QueueFade.Count > 0)
		{
			m_IsDoing = true;
			FadeInfo fadeInfo = m_QueueFade.Dequeue();

			m_Component.imgShade.DOKill();
			m_Component.imgShade.color = new Color(0, 0, 0, fadeInfo.from);
			m_Component.imgShade.DOFade(fadeInfo.to, fadeInfo.duration).SetDelay(fadeInfo.delay).OnComplete(() =>
			{
				fadeInfo.onComplete?.Invoke();
				StartDoFade();
			});
		}
		else
		{
			m_IsDoing = false;
        }
    }


	protected override void OnClose()
	{
		m_IsDoing = false;
	}

	protected override void OnDestroy()
	{

	}

	private bool m_IsDoing = false;
	private Queue<FadeInfo> m_QueueFade = null;
	private LoadPanelComponent m_Component = null;
}