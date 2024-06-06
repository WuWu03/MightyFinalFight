using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTaskTrigger : ITaskTrigger
{
    public bool isComplete
    {
        get
        {
            return m_IsComplete;
        }
    }

    public BaseTaskTrigger(TaskConfigData data)
    {
        m_TaskData = data;
    }

    public virtual void Enter()
    {
        m_IsComplete = false;
    }

    public virtual void Trigger()
    {
        if(m_TaskData.TriggerPlayerCantCtrl)
        {
            PlayerMgr.instance.canContrl = false;
        }

        if (m_TaskData.TriggerStopCamera)
        {
            CameraMgr.instance.EndFollow(true);
        }
    }

    public virtual void Complete()
    {
        m_IsComplete = true;
    }

    private bool m_IsComplete = false;
    protected TaskConfigData m_TaskData = null;
}
