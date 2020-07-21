using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskFactory
{
    public static BaseTask CreateTask(TaskData data)
    {
        BaseTask ret = null;
        if (data.TriggerCondition == TaskData.ConditionType.MoveToPos)
            ret = new TaskMoveToPos(data);
        else if (data.TriggerCondition == TaskData.ConditionType.KillEnemy)
            ret = new TaskKillEnemy(data);
        else if (data.TriggerCondition == TaskData.ConditionType.WaitBarrels)
            ret = new TaskWaitBarrels(data);
        return ret;
    }

    public static BaseTaskTrigger CreateTaskTrigger(TaskData data)
    {
        BaseTaskTrigger ret = null;
        if (data.TriggerEffect == TaskData.EffectType.Enemy)
            ret = new TaskTriggerEnemy(data);
        return ret;
    }
}
