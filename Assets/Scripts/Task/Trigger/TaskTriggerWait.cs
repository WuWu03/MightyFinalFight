using GameFrameWork.Camera;
using UnityEngine;

public class TaskTriggerWait : BaseTaskTrigger
{
    public TaskTriggerWait(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();
        m_WaitTimer = Time.time;
    }

    public override void Trigger()
    {
        if(Time.time - m_WaitTimer >= m_TaskData.WaitTime)
        {
            Complete();
        }
    }

    private float m_WaitTimer = 0f;
}