public class TaskTriggerStory : BaseTaskTrigger
{
    public TaskTriggerStory(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        PlayerMgr.instance.canContrl = false;
        StoryMgr.instance.onPlayCompleteEvent += OnStoryComplete;
        StoryMgr.instance.Play(m_TaskData.StoryId);
    }

    private void OnStoryComplete()
    {
        PlayerMgr.instance.canContrl = true;
        Complete();
    }
}
