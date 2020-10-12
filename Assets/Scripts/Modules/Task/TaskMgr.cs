using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class TaskMgr : BaseMgr<TaskMgr>
{
    private void Awake()
    {
        m_CurrTaskList = new List<BaseTask>();
        m_CurrTaskIndex = 0;
        m_LastTaskIndex = -1;
    }

    public void AcceptTask(int id)
    {
        if(HasAccepted(id))
        {
            return;
        }

        m_CurrTaskList.Add(TaskFactory.CreateTask(StaticConfig.TaskConfig.GetData(id)));
    }

    public bool HasAccepted(int id)
    {
        for (int i = 0; i < m_CurrTaskList.Count; i++)
        {
            if (m_CurrTaskList[i].TaskData.ID.Equals(id))
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (m_CurrTaskList == null || m_CurrTaskList.Count < 1 || m_CurrTaskIndex >= m_CurrTaskList.Count) return;

        if (!m_CurrTaskList[m_CurrTaskIndex].IsComplete)
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
        }
        else
        {
            if(m_CurrTaskList[m_CurrTaskIndex].Exit())
            {
                if (m_CurrTaskList[m_CurrTaskIndex].TaskData.NextID != 0)
                    AcceptTask(m_CurrTaskList[m_CurrTaskIndex].TaskData.NextID);
                m_CurrTaskIndex++;
            }
        }
    }


    private int m_CurrTaskIndex = 0;
    private int m_LastTaskIndex = -1;
    private List<BaseTask> m_CurrTaskList = null;
}
