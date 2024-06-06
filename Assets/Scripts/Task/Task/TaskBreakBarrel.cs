using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBreakBarrel : BaseTask
{
    public TaskBreakBarrel(TaskConfigData data) : base(data) { }
    public override bool CheckCondition()
    {
        if (m_TaskData.KillAllBarrels)
        {
            return SceneEntityMgr.instance.IsAllBarrelsBreak();
        }

        if (m_TaskData.KillEnemyIDs.Length < 1)
        {
            return true;
        }

        for (int i = 0; i < m_TaskData.KillEnemyIDs.Length; i++)
        {
            if (!SceneEntityMgr.instance.IsBarrelBreak(m_TaskData.KillEnemyIDs[i]))
            {
                return false;
            }
        }

        return true;
    }
}
