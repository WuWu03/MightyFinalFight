using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTask
{
    public TaskConfigData TaskData
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

    public BaseTask(TaskConfigData data)
    {
        m_TaskData = data;
        m_IsComplete = false;
        m_Trigger = TaskFactory.CreateTaskTrigger(data);
    }

    public virtual void Enter()
    {

    }
    public virtual void Update()
    {
        if (m_Trigger != null && m_Trigger.IsComplete)
            m_IsComplete = true;
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
            PlayerMgr.Ins.CanContrl = false;
            PlayerMgr.Ins.SetSpeedZero(true);
            CameraMgr.Ins.StartFollow(true);
            float cameraX = CameraMgr.Ins.CameraRoot.transform.position.x;
            float playerX = PlayerMgr.Ins.Player.Pos.x;
            bool isDistance = cameraX >= playerX || Mathf.Abs(cameraX - playerX) <= 0.03f;

            if (isDistance)
            {
                PlayerMgr.Ins.CanContrl = true;
                PlayerMgr.Ins.SetSpeedZero(false);
            }
            return isDistance;
        }

        return true;
    }

    protected TaskConfigData m_TaskData = null;
    private bool m_IsComplete = false;
    private BaseTaskTrigger m_Trigger = null;
}
