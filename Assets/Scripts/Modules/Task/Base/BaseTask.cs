using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTask
{
    public TaskConfigData taskData
    {
        get
        {
            return m_TaskData;
        }
    }

    public bool isComplete
    {
        get
        {
            return m_IsComplete;
        }
    }

    public BaseTask(TaskConfigData data)
    {
        m_TaskData = data;
        m_IsComplete = false;
        m_Trigger = TaskFactory.CreateTaskTrigger(data);
    }

    public virtual void Enter()
    {
        if (m_Trigger != null)
        {
            m_Trigger.Enter();
        }
    }

    public virtual void Update()
    {
        if (m_Trigger != null && m_Trigger.isComplete)
        {
            m_IsComplete = true;
        }
    }

    public abstract bool CheckCondition();

    public virtual void Trigger()
    {
        if (!m_IsComplete)
        {
            if (m_Trigger != null) m_Trigger.Trigger();
            else m_IsComplete = true;
        }
    }

    public virtual bool Exit()
    {
        if (m_TaskData.ExitStartCamera)
        {
            PlayerMgr.instance.canContrl = false;
            PlayerMgr.instance.SetSpeedZero(true);
            CameraMgr.instance.SetFollowMode(FollowMode.Linear);
            CameraMgr.instance.StartFollow(true);
            float cameraX = CameraMgr.instance.cameraRoot.transform.position.x;
            float playerX = PlayerMgr.instance.player.pos.x;
            bool isDistance = cameraX >= playerX || Mathf.Abs(cameraX - playerX) <= 0f;

            if (isDistance)
            {
                PlayerMgr.instance.canContrl = true;
                PlayerMgr.instance.RevertSpeed();
                CameraMgr.instance.SetFollowMode(FollowMode.Just);
            }
            return isDistance;
        }

        if (m_TaskData.ExitPlayerCanCtrl)
        {
            PlayerMgr.instance.canContrl = true;
        }

        return true;
    }

    protected TaskConfigData m_TaskData = null;
    private bool m_IsComplete = false;
    private BaseTaskTrigger m_Trigger = null;
}
