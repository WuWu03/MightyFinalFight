using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskFactory
{
    public static BaseTask CreateTask(TaskConfigData data)
    {
        BaseTask ret = null;
        if (data.ConditionType == TaskConfigData.TaskConditionType.MoveToPos)
            ret = new TaskMoveToPos(data);
        else if (data.ConditionType == TaskConfigData.TaskConditionType.KillEnemy)
            ret = new TaskKillEnemy(data);
        else if (data.ConditionType == TaskConfigData.TaskConditionType.WaitBarrels)
            ret = new TaskWaitBarrels(data);
        else if (data.ConditionType == TaskConfigData.TaskConditionType.AutoMoveToPos)
            ret = new TaskAutoMoveToPos(data);
        return ret;
    }

    public static BaseTaskTrigger CreateTaskTrigger(TaskConfigData data)
    {
        BaseTaskTrigger ret = null;
        if (data.TriggerType == TaskConfigData.TaskTriggerType.Enemy)
            ret = new TaskTriggerEnemy(data);
        else if (data.TriggerType == TaskConfigData.TaskTriggerType.Finger)
            ret = new TaskTriggerFinger(data);
        else if (data.TriggerType == TaskConfigData.TaskTriggerType.ChangeScene)
            ret = new TaskTriggerChangeScene(data);
        return ret;
    }
}
