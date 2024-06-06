/*******************************************************/
/**2021-7-23 10:03**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadPanel : BasePanel
{
	class FadeInfo
	{
		public float from;
		public float to;
		public float duration;
        public float delay;
		public GameFrameWorkAction onComplete;
    }

    protected override Type componentType
    {
		get
		{
			return typeof(LoadPanelComponent);
		}
    }

    protected override Type settingsType
    {
        get
        {
            return typeof(LoadPanelSettings);
        }
    }

    protected override void OnInit(BasePanelComponent panelComponent, object[] param)
    {
        m_Component = panelComponent as LoadPanelComponent;
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