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
        GameEntry.uiMgr.Open<TalkPresenter>(taskConfigData.TalkID);
        PlayerMgr.instance.player.Move(Vector2.zero);
        GameEntry.eventMgr.Subscribe<TalkEndEvent>(OnTalkEnd);
    }

    private void OnTalkEnd(object sender, TalkEndEvent e)
    {
        Complete();
    }

    public override void Complete()
    {
        base.Complete();
        GameEntry.eventMgr.UnSubscribe<TalkEndEvent>(OnTalkEnd);
    }
}
