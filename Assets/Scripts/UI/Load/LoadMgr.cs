using System.Collections.Generic;
using WuWuFramework;
using WuWuFramework.Event;

public class LoadMgr : Singleton<LoadMgr>
{
    public class FadeArgs : WuWuFrameworkEventArg
    {
        public float from;
        public float to;
        public float duration;
        public float delay;
        public WuWuFrameworkAction onComplete;

        public static FadeArgs Create(float from, float to, float duration, float delay, WuWuFrameworkAction onComplete)
        {
            FadeArgs fadeInfo = ReferencePool.Acquire<FadeArgs>();
            fadeInfo.from = from;
            fadeInfo.to = to;
            fadeInfo.duration = duration;
            fadeInfo.delay = delay;
            fadeInfo.onComplete = onComplete;
            return fadeInfo;
        }

        public override void Clear()
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

    public LoadMgr()
    {
        m_QueueFade = new();
    }

    protected override void OnShutdown()
    {
        m_QueueFade.Clear();
    }

    public FadeArgs GetFadeInfo()
    {
        if (m_QueueFade.Count > 0)
        {
            return m_QueueFade.Dequeue();
        }
        
        return null;
    }

    public void DOFadeBlack(WuWuFrameworkAction onComplete, float duration = 0.3f, float delay = 0.5f)
    {
        DOFade(0, 1, duration, delay, onComplete);
    }

    public void DOFadeWhite(WuWuFrameworkAction onComplete, float duration = 0.3f, float delay = 0.5f)
    {
        DOFade(1, 0, duration, delay, onComplete);
    }

    public void DOFade(float from, float to, float duration, float delay, WuWuFrameworkAction onComplete)
    {
        m_IsFadeComplete = false;

        lock (m_QueueFade)
        {
            m_QueueFade.Enqueue(FadeArgs.Create(from, to, duration, delay, onComplete));
        }

        GameEntry.uiMgr.Open<LoadView>();
    }

    public void CloseLoadPanel()
    {
        GameEntry.uiMgr.Close<LoadView>();
    }

    private bool m_IsFadeComplete = false;
    private Queue<FadeArgs> m_QueueFade = null;
}
