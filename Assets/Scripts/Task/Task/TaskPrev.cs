public class TaskPrev : BaseTask
{
    public TaskPrev(TaskConfigData data) : base(data) { }

    public override bool CheckCondition()
    {
        return TaskMgr.instance.TaskHasCompleted(m_TaskData.PrevID);
    }
}
