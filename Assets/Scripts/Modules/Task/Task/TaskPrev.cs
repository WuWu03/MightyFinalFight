using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPrev : BaseTask
{
    public TaskPrev(TaskConfigData data) : base(data) { }

    public override bool CheckCondition()
    {
        return TaskMgr.Ins.TaskHasCompleted(m_TaskData.PrevID);
    }
}
