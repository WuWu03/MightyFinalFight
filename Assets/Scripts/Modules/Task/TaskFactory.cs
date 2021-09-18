using GameFrameWork.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                ret = new TaskMoveToPos(data);
                break;
            case TaskConfigData.TaskConditionType.KillEnemy:
                ret = new TaskKillEnemy(data);
                break;
            case TaskConfigData.TaskConditionType.WaitBarrels:
                ret = new TaskWaitBarrels(data);
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
            case TaskConfigData.TaskTriggerType.Enemy:
                ret = new TaskTriggerEnemy(data);
                break;
            case TaskConfigData.TaskTriggerType.Barrels:
                break;
            case TaskConfigData.TaskTriggerType.Story:
                string className = TextUtil.FormatDefault("TaskTriggerStory_", data.StoryId);
                Type t = Type.GetType(className);
                if(t != null)
                {
                    ret = (BaseTaskTrigger)System.Activator.CreateInstance(t, data);
                }
                break;
            case TaskConfigData.TaskTriggerType.Finger:
                ret = new TaskTriggerFinger(data);
                break;
            case TaskConfigData.TaskTriggerType.ChangeScene:
                ret = new TaskTriggerChangeScene(data);
                break;
            case TaskConfigData.TaskTriggerType.AutoMoveToPos:
                ret = new TaskTriggerAutoMoveToPos(data);
                break;
        }

        return ret;
    }
}
