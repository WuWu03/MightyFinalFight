using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerEnemy : BaseTaskTrigger
{
    public TaskTriggerEnemy(TaskData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        for (int i = 0; i < m_TaskData.Targets.Length; i++)
        {
            StageMgr.Ins.CreateEnemy(m_TaskData.Targets[i].ID, m_TaskData.Targets[i].Pos);
        }

        m_IsComplete = true;
    }
}
