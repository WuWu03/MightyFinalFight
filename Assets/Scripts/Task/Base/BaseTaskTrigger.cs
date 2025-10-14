using GameFrameWork.Camera;

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
        mTaskData = data;
    }

    public virtual void Enter()
    {
        m_IsComplete = false;
    }

    public virtual void Trigger()
    {
        if(mTaskData.TriggerPlayerCantCtrl)
        {
            PlayerMgr.instance.canContrl = false;
        }

        if (mTaskData.TriggerStopCamera)
        {
            CameraMgr.instance.EndFollow(true);
        }
    }

    public virtual void Complete()
    {
        m_IsComplete = true;
    }

    private bool m_IsComplete = false;
    protected TaskConfigData mTaskData = null;
}
