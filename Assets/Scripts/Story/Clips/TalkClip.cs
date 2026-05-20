using GameFrameWork;
using GameFrameWork.Event;

public class TalkClip : BaseClip
{
    private int m_TalkId;
    public static TalkClip Create(int talkId)
    {
        TalkClip clip = ReferencePool.Acquire<TalkClip>();
        clip.m_TalkId = talkId;
        return clip;
    }

    protected override void OnClear()
    {
        m_TalkId = 0;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        GameEntry.eventMgr.Subscribe<TalkEndEvent>(OnTalkEnd);
        GameEntry.uiMgr.Open<TalkPresenter>(m_TalkId);
    }

    private void OnTalkEnd(object sender, TalkEndEvent e)
    {
        GameEntry.eventMgr.UnSubscribe<TalkEndEvent>(OnTalkEnd);
        Complete();
    }

    protected override void OnResume()
    {

    }
}
