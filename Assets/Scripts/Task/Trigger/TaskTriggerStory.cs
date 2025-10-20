public class TaskTriggerStory : BaseTaskTrigger
{
    public TaskTriggerStory(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        PlayerMgr.instance.canControl = false;
        StoryMgr.instance.onPlayCompleteEvent += OnStoryComplete;
        StoryMgr.instance.Play(taskConfigData.StoryId);
    }

    private void OnStoryComplete()
    {
        PlayerMgr.instance.canControl = true;
        Complete();
    }
}
