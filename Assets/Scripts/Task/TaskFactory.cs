public static class TaskFactory
{
    public static BaseTask CreateTask(TaskConfigData data)
    {
        BaseTask ret = null;
        switch (data.ConditionType)
        {
            case TaskConfigData.TaskConditionType.None:
                ret = new TaskNone(data);
                break;
            case TaskConfigData.TaskConditionType.MoveToPos:
                ret = new TaskMove(data);
                break;
            case TaskConfigData.TaskConditionType.KillTarget:
                ret = new TaskKillEnemy(data);
                break;
            case TaskConfigData.TaskConditionType.PrevTask:
                ret = new TaskPrev(data);
                break;
        }

        return ret;
    }

    public static BaseTaskTrigger CreateTaskTrigger(TaskConfigData data)
    {
        BaseTaskTrigger ret = null;

        switch (data.TriggerType)
        {
            case TaskConfigData.TaskTriggerType.None:
                break;
            case TaskConfigData.TaskTriggerType.CreateTargets:
                ret = new TaskTriggerCreateTargets(data);
                break;
            case TaskConfigData.TaskTriggerType.Finger:
                ret = new TaskTriggerFinger(data);
                break;
            case TaskConfigData.TaskTriggerType.ChangeScene:
                ret = new TaskTriggerChangeScene(data);
                break;
            case TaskConfigData.TaskTriggerType.AutoMoveToPos:
                ret = new TaskTriggerAutoMove(data);
                break;
            case TaskConfigData.TaskTriggerType.Talk:
                ret = new TaskTriggerTalk(data);
                break;
            case TaskConfigData.TaskTriggerType.RoundClear:
                ret = new TaskTriggerRoundClear(data);
                break;
            case TaskConfigData.TaskTriggerType.Story:
                ret = new TaskTriggerStory(data);
                break;
        }

        return ret;
    }
}
