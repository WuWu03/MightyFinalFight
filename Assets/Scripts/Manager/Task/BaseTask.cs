using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public abstract class BaseTask
{
    public TaskData TaskData
    {
        get
        {
            return m_TaskData;
        }
    }

    public bool IsComplete
    {
        get
        {
            return m_IsComplete;
        }
    }

    public BaseTask(TaskData data)
    {
        m_TaskData = data;
        m_IsComplete = false;
    }

    public abstract void Update();
    public abstract bool CheckCondition();

    public abstract void Trigger();

    private bool m_IsComplete = false;
    private TaskData m_TaskData = null;
}
