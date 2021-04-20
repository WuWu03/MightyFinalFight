using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTaskTrigger : ITaskTrigger
{
    public bool IsComplete
    {
        get
        {
            return m_IsComplete;
        }
    }

    public BaseTaskTrigger(TaskData data)
    {
        m_TaskData = data;
    }

    public virtual void Trigger()
    {
        if(m_TaskData.TriggerStopCamera)
        {
            CameraMgr.Ins.EndFollow(true);
        }
    }

    protected bool m_IsComplete = false;
    protected TaskData m_TaskData = null;
}
