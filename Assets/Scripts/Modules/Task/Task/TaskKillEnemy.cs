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
            return SceneEntityMgr.Ins.IsAllEnemyDead();
        }

        if (m_TaskData.KillIDs.Length < 1)
        {
            return true;
        }

        for (int i = 0; i < m_TaskData.KillIDs.Length; i++)
        {
            if (!SceneEntityMgr.Ins.IsEnemyDead(m_TaskData.KillIDs[i]))
            {
                return false;
            }
        }

        return true;
    }
}
