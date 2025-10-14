public class TaskKillEnemy : BaseTask
{
    public TaskKillEnemy(TaskConfigData data) : base(data)
    {
        m_AllConditions = new bool[m_ConditionCount];
        m_Results = new bool[m_ConditionCount];
    }

    public override bool CheckCondition()
    {
        for (int i = 0; i < m_ConditionCount; i++)
        {
            m_AllConditions[i] = false;
            m_Results[i] = false;
        }

        if (mTaskData.KillAllEnemies)
        {
            m_AllConditions[0] = true;
            m_Results[0] = SceneEntityMgr.instance.IsAllEnemyDead();
        }

        if (mTaskData.KillAllBarrels)
        {
            m_AllConditions[1] = true;
            m_Results[1] = SceneEntityMgr.instance.IsAllBarrelsBreak();
        }

        if (mTaskData.KillEnemyIDs.Length > 0)
        {
            m_AllConditions[2] = true;
            m_Results[2] = true;

            for (int i = 0; i < mTaskData.KillEnemyIDs.Length; i++)
            {
                if (!SceneEntityMgr.instance.IsEnemyDead(mTaskData.KillEnemyIDs[i]))
                {
                    m_Results[2] = false;
                    break;
                }
            }
        }

        if (mTaskData.BreakBarrelIDs.Length > 0)
        {
            m_AllConditions[3] = true;
            m_Results[3] = true;

            for (int i = 0; i < mTaskData.BreakBarrelIDs.Length; i++)
            {
                if (!SceneEntityMgr.instance.IsBarrelBreak(mTaskData.BreakBarrelIDs[i]))
                {
                    m_Results[3] = false;
                    break;
                }
            }
        }

        for (int i = 0; i < m_AllConditions.Length; i++)
        {
            if (m_AllConditions[i] && !m_Results[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool[] m_AllConditions = null;
    private bool[] m_Results = null;
    private int m_ConditionCount = 4;
}
