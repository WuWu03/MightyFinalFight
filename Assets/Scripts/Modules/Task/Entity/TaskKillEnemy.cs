using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskKillEnemy : BaseTask
{
    public TaskKillEnemy(TaskConfigData data) : base(data) { }

    public override bool CheckCondition()
    {
        if (m_TaskData.KillAll)
        {
            m_IsComplete = SceneEntityMgr.Ins.IsAllEnemyDead();
            return m_IsComplete;
        }

        if (m_TaskData.KillIDs.Length < 1)
        {
            m_IsComplete = true;
            return true;
        }

        for (int i = 0; i < m_TaskData.KillIDs.Length; i++)
        {
            if (!SceneEntityMgr.Ins.IsEnemyDead(m_TaskData.KillIDs[i]))
            {
                m_IsComplete = false;
                return false;
            }
        }

        m_IsComplete = true;
        return true;
    }
}
