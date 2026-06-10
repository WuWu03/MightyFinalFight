using UnityEngine;

public abstract class BaseTask : ITask
{
    public TaskConfigData TaskData
    {
        get
        {
            return mTaskData;
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
        mTaskData = data;
        m_IsComplete = false;
        m_Trigger = TaskFactory.CreateTaskTrigger(data);
    }

    public virtual void Enter()
    {
        m_Trigger?.Enter();
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
            if (m_Trigger != null)
            {
                m_Trigger.Trigger();
            }

            else m_IsComplete = true;
        }
    }

    public virtual bool CanComplete()
    {
        if (mTaskData.ExitStartCamera)
        {
            PlayerMgr.instance.canControl = false;
            PlayerMgr.instance.SetSpeedZero();
            CameraFollowMgr.instance.cameraFollow.followMode = CameraFollow.FollowMode.Linear;
            CameraFollowMgr.instance.cameraFollow.StartFollow(true);
            float cameraX = CameraFollowMgr.instance.cameraFollow.transform.position.x;
            float playerX = PlayerMgr.instance.player.pos.x;
            bool isDistance = cameraX >= playerX || Mathf.Abs(cameraX - playerX) <= 0f;

            if (isDistance)
            {
                PlayerMgr.instance.canControl = true;
                PlayerMgr.instance.RevertSpeed();
                CameraFollowMgr.instance.cameraFollow.followMode = CameraFollow.FollowMode.Just;
            }

            return isDistance;
        }

        if (mTaskData.ExitPlayerCanCtrl)
        {
            PlayerMgr.instance.canControl = true;
        }

        return true;
    }

    protected TaskConfigData mTaskData = null;
    private bool m_IsComplete = false;
    private readonly BaseTaskTrigger m_Trigger = null;
}
