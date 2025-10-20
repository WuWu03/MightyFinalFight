public class TaskTriggerChangeScene : BaseTaskTrigger
{
    public TaskTriggerChangeScene(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();
        PlayerMgr.instance.canControl = false;
    }

    public override void Trigger()
    {
        base.Trigger();
        StageMgr.instance.StageEnter(taskConfigData.MapID);
        Complete();
    }
}