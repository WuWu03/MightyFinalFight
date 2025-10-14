using GameFrameWork;
using GameFrameWork.Event;
using GameFrameWork.UI;

public class TalkClip : BaseClip
{
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
        EventMgr.instance.Subscribe(EventId.TalkEndEvent, OnTalkEnd);
        UIMgr.instance.Open<TalkView>(m_TalkId);
    }

    private void OnTalkEnd(object sender, GameEventArg e)
    {
        EventMgr.instance.UnSubscribe(EventId.TalkEndEvent, OnTalkEnd);
        Complete();
    }

    protected override void OnResume()
    {

    }

    private int m_TalkId = 0;
}
