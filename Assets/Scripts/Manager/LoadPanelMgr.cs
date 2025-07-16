using GameFrameWork;
using GameFrameWork.Event;
using GameFrameWork.UI;
using System.Collections.Generic;


public class LoadPanelMgr : BaseMgr<LoadPanelMgr>
{
    public class FadeInfo : IReference
    {
        public float from;
        public float to;
        public float duration;
        public float delay;
        public GameFrameWorkAction onComplete;

        public static FadeInfo Create(float from, float to, float duration, float delay, GameFrameWorkAction onComplete)
        {
            FadeInfo fadeInfo = ReferencePool.Acquire<FadeInfo>();
            fadeInfo.from = from;
            fadeInfo.to = to;
            fadeInfo.duration = duration;
            fadeInfo.delay = delay;
            fadeInfo.onComplete = onComplete;
            return fadeInfo;
        }

        public void Clear()
        {
            from = 0;
            to = 0;
            duration = 0;
            delay = 0;
            onComplete = null;
        }
    }

    public bool isComplete
    {
        get
        {
            return m_IsFadeComplete;
        }
    }

    public int fadeInfoCount
    {
        get
        {
            return m_QueueFade.Count;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_QueueFade = new Queue<FadeInfo>();
    }

    protected override void OnShutDown()
    {
        base.OnShutDown();
        m_QueueFade.Clear();
        m_QueueFade = null;
    }

    public FadeInfo GetFadeInfo()
    {
        if (m_QueueFade.Count > 0)
        {
            return m_QueueFade.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public void DOFadeBlack(GameFrameWorkAction onComplete, float duration = 0.3f, float delay = 0.5f)
    {
        DOFade(0, 1, duration, delay, onComplete);
    }

    public void DOFadeWhite(GameFrameWorkAction onComplete, float duration = 0.3f, float delay = 0.5f)
    {
        DOFade(1, 0, duration, delay, onComplete);
    }

    public void DOFade(float from, float to, float duration, float delay, GameFrameWorkAction onComplete)
    {
        m_IsFadeComplete = false;

        lock (m_QueueFade)
        {
            m_QueueFade.Enqueue(FadeInfo.Create(from, to, duration, delay, onComplete));
        }

        UIMgr.instance.Open(UINames.LoadPanel);
    }

    public void CloseLoadPanel()
    {
        UIMgr.instance.Close(UINames.LoadPanel);
    }

    private bool m_IsFadeComplete = false;
    private Queue<FadeInfo> m_QueueFade = null;
}
