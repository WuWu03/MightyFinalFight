using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskFactory
{
    public static BaseTask CreateTask(TaskConfigData data)
    {
        BaseTask ret = null;
        if (data.TriggerCondition == TaskConfigData.ConditionType.MoveToPos)
            ret = new TaskMoveToPos(data);
        else if (data.TriggerCondition == TaskConfigData.ConditionType.KillEnemy)
            ret = new TaskKillEnemy(data);
        else if (data.TriggerCondition == TaskConfigData.ConditionType.WaitBarrels)
            ret = new TaskWaitBarrels(data);
        return ret;
    }

    public static BaseTaskTrigger CreateTaskTrigger(TaskConfigData data)
    {
        BaseTaskTrigger ret = null;
        if (data.TriggerEffect == TaskConfigData.EffectType.Enemy)
            ret = new TaskTriggerEnemy(data);
        return ret;
    }
}
