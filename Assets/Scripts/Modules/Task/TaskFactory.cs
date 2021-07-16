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
        return ret;
    }

    public static BaseTaskTrigger CreateTaskTrigger(TaskConfigData data)
    {
        BaseTaskTrigger ret = null;
        if (data.TriggerType == TaskConfigData.TaskTriggerType.Enemy)
            ret = new TaskTriggerEnemy(data);
        if (data.TriggerType == TaskConfigData.TaskTriggerType.Finger)
            ret = new TaskTriggerFinger(data);
        return ret;
    }
}
