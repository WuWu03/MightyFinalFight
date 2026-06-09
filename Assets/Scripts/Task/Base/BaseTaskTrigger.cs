public abstract class BaseTaskTrigger : ITaskTrigger
{
    private bool m_IsComplete;
    private readonly TaskConfigData m_TaskData;
    
    public bool isComplete
    {
        get
        {
            return m_IsComplete;
        }
    }

    public TaskConfigData taskConfigData
    {
        get
        {
            return m_TaskData;
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
            PlayerMgr.instance.canControl = false;
        }

        if (m_TaskData.TriggerStopCamera)
        {
            CameraMgr.instance.cameraFollow.EndFollow(true);
        }
    }

    public virtual void Complete()
    {
        m_IsComplete = true;
    }
}
