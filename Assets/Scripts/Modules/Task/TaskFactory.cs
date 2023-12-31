using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            case TaskConfigData.TaskTriggerType.Story:
                string className = StringUtil.Format("TaskTriggerStory_", data.StoryId);
                Type t = Type.GetType(className);
                if (t != null)
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
            case TaskConfigData.TaskTriggerType.Talk:
                ret = new TaskTriggerTalk(data);
                break;
            case TaskConfigData.TaskTriggerType.RoundClear:
                ret = new TaskTriggerRoundClear(data);
                break;
        }

        return ret;
    }
}
