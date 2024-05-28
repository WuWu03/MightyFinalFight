using GameFrameWork;
using System.Collections.Generic;

public class TaskMgr : BaseMgr<TaskMgr>
{
    protected override void OnAwake()
    {
        m_CurrTaskList = new List<BaseTask>();
        m_CompleteTask = new List<BaseTask>();
        m_FailureIdList = new List<int>();
        m_CurrTaskIndex = 0;
        m_LastTaskIndex = -1;
    }

    public void AcceptTask(int id)
    {
        if (TaskHasAccepted(id) || TaskHasCompleted(id))
        {
            return;
        }

        if (TaskHasFailure(id, true))
        {
            m_CompleteTask.Add(TaskFactory.CreateTask(StaticConfig.TaskConfig.GetData(id)));
        }
        else
        {
            m_CurrTaskList.Add(TaskFactory.CreateTask(StaticConfig.TaskConfig.GetData(id)));
        }
    }

    public bool TaskHasAccepted(int id)
    {
        for (int i = 0; i < m_CurrTaskList.Count; i++)
        {
            if (m_CurrTaskList[i].taskData.id.Equals(id))
            {
                return true;
            }
        }

        return false;
    }

    public bool TaskHasCompleted(int id)
    {
        for (int i = 0; i < m_CompleteTask.Count; i++)
        {
            if (m_CompleteTask[i].taskData.id.Equals(id))
            {
                return true;
            }
        }

        return false;
    }

    public void GiveupTask()
    {
        m_CurrTaskList.Clear();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_CurrTaskList == null || m_CurrTaskList.Count < 1)
        {
            return;
        }

        if (!m_CurrTaskList[m_CurrTaskIndex].isComplete)
        {
            m_CurrTaskList[m_CurrTaskIndex].Update();

            if (m_CurrTaskList[m_CurrTaskIndex].CheckCondition())
            {
                if (m_CurrTaskIndex != m_LastTaskIndex)
                {
                    m_CurrTaskList[m_CurrTaskIndex].Enter();
                    m_LastTaskIndex = m_CurrTaskIndex;
                }

                m_CurrTaskList[m_CurrTaskIndex].Trigger();
            }
            else
            {
                NextTask();
            }
        }
        else
        {
            if(m_CurrTaskList[m_CurrTaskIndex].Exit())
            {
                CompleteTask();
            }
        }
    }

    private void NextTask()
    {
        m_CurrTaskIndex++;
        m_LastTaskIndex = -1;

        if (m_CurrTaskIndex >= m_CurrTaskList.Count)
        {
            m_CurrTaskIndex = 0;
        }
    }

    private void CompleteTask()
    {
        int nextId = m_CurrTaskList[m_CurrTaskIndex].taskData.NextID;
        int failureId = m_CurrTaskList[m_CurrTaskIndex].taskData.FailureID;

        m_CompleteTask.Add(m_CurrTaskList[m_CurrTaskIndex]);
        m_CurrTaskList.RemoveAt(m_CurrTaskIndex);
        m_LastTaskIndex = -1;

        if (m_CurrTaskList.Count < 1)
        {
            m_CurrTaskIndex = 0;
        }
        else if (m_CurrTaskIndex >= m_CurrTaskList.Count)
        {
            m_CurrTaskIndex = m_CurrTaskList.Count - 1;
        }

        if (nextId != 0)
        {
            AcceptTask(nextId);
        }

        if (failureId != 0)
        {
            bool hasFailure = false;

            for (int i = 0; i < m_CurrTaskList.Count; i++)
            {
                if(m_CurrTaskList[i].taskData.id.Equals(failureId))
                {
                    hasFailure = true;
                    m_CompleteTask.Add(m_CurrTaskList[i]);
                    m_CurrTaskList.RemoveAt(m_CurrTaskIndex);
                    break;
                }
            }

            if(!hasFailure)
            {
                m_FailureIdList.Add(failureId);
            }
        }
    }

    private bool TaskHasFailure(int id, bool remove = false)
    {
        if(m_FailureIdList == null || m_FailureIdList.Count < 1)
        {
            return false;
        }

        for (int i = 0; i < m_FailureIdList.Count; i++)
        {
            if(m_FailureIdList[i].Equals(id))
            {
                if (remove)
                {
                    m_FailureIdList.RemoveAt(i);
                }
                return true;
            }
        }

        return false;
    }

    private int m_CurrTaskIndex = 0;
    private int m_LastTaskIndex = -1;
    private List<int> m_FailureIdList = null;
    private List<BaseTask> m_CurrTaskList = null;
    private List<BaseTask> m_CompleteTask = null;
}
