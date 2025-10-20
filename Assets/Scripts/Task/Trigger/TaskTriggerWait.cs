using UnityEngine;

public class TaskTriggerWait : BaseTaskTrigger
{
    private float m_WaitTimer;
    public TaskTriggerWait(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();
        m_WaitTimer = Time.time;
    }

    public override void Trigger()
    {
        if(Time.time - m_WaitTimer >= taskConfigData.WaitTime)
        {
            Complete();
        }
    }
}