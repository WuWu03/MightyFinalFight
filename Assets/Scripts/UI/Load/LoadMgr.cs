using GameFrameWork;
using GameFrameWork.UI;
using System.Collections.Generic;
using GameFrameWork.Event;


public class LoadMgr : BaseMgr<LoadMgr>
{
    public class FadeArgs : GameFrameWorkEventArg
    {
        public float from;
        public float to;
        public float duration;
        public float delay;
        public GameFrameWorkAction onComplete;

        public static FadeArgs Create(float from, float to, float duration, float delay, GameFrameWorkAction onComplete)
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

    protected override void OnAwake()
    {
        base.OnAwake();
        m_QueueFade = new Queue<FadeArgs>();
    }

    protected override void OnShutDown()
    {
        base.OnShutDown();
        m_QueueFade.Clear();
    }

    protected override void OnDestory()
    {
        base.OnDestory();
        m_QueueFade = null;
    }

    public FadeArgs GetFadeInfo()
    {
        if (m_QueueFade.Count > 0)
        {
            return m_QueueFade.Dequeue();
        }
        
        return null;
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
            m_QueueFade.Enqueue(FadeArgs.Create(from, to, duration, delay, onComplete));
        }

        GameFrameWorkMgr.GetModule<IUIMgr>().Open<LoadView>();
    }

    public void CloseLoadPanel()
    {
        GameFrameWorkMgr.GetModule<IUIMgr>().Close<LoadView>();
    }

    private bool m_IsFadeComplete = false;
    private Queue<FadeArgs> m_QueueFade = null;
}
