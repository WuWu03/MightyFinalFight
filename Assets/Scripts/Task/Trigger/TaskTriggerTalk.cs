using GameFrameWork.Event;
using GameFrameWork.UI;
using UnityEngine;

public class TaskTriggerTalk : BaseTaskTrigger
{
    // Start is called before the first frame update
    public TaskTriggerTalk(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        base.Enter();
        UIMgr.instance.Open<TalkView>(mTaskData.TalkID);
        PlayerMgr.instance.player.Move(Vector2.zero);
        EventMgr.instance.Subscribe(EventId.TalkEndEvent, OnTalkEnd);
    }

    private void OnTalkEnd(object sender, GameEventArg e)
    {
        Complete();
    }

    public override void Complete()
    {
        base.Complete();
        EventMgr.instance.UnSubscribe(EventId.TalkEndEvent, OnTalkEnd);
    }
}
