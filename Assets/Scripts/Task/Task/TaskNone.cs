public class TaskNone : BaseTask
{
    public TaskNone(TaskConfigData data) : base(data)
    {

    }

    public override bool CheckCondition()
    {
        return true;
    }
}
