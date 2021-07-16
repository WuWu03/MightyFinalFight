using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerEnemy : BaseTaskTrigger
{
    public TaskTriggerEnemy(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        for (int i = 0; i < m_TaskData.Targets.Length; i++)
        {
            SceneEntityMgr.Ins.CreateEnemy(m_TaskData.Targets[i].SourceID, m_TaskData.Targets[i].EntityID, m_TaskData.Targets[i].Pos);
        }

        SceneEntityMgr.Ins.CreateBarrels();
        m_IsComplete = true;
    }
}