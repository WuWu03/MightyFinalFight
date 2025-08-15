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

        if (m_TaskData.KillAllEnemies)
        {
            m_AllConditions[0] = true;
            m_Results[0] = SceneEntityMgr.instance.IsAllEnemyDead();
        }

        if (m_TaskData.KillAllBarrels)
        {
            m_AllConditions[1] = true;
            m_Results[1] = SceneEntityMgr.instance.IsAllBarrelsBreak();
        }

        if (m_TaskData.KillEnemyIDs.Length > 0)
        {
            m_AllConditions[2] = true;
            m_Results[2] = true;

            for (int i = 0; i < m_TaskData.KillEnemyIDs.Length; i++)
            {
                if (!SceneEntityMgr.instance.IsEnemyDead(m_TaskData.KillEnemyIDs[i]))
                {
                    m_Results[2] = false;
                    break;
                }
            }
        }

        if (m_TaskData.BreakBarrelIDs.Length > 0)
        {
            m_AllConditions[3] = true;
            m_Results[3] = true;

            for (int i = 0; i < m_TaskData.BreakBarrelIDs.Length; i++)
            {
                if (!SceneEntityMgr.instance.IsBarrelBreak(m_TaskData.BreakBarrelIDs[i]))
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
