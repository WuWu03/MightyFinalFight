

using UnityEngine;

public class TaskTriggerChangeScene : BaseTaskTrigger
{
    public TaskTriggerChangeScene(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();
        PlayerMgr.instance.canContrl = false;
    }

    public override void Trigger()
    {
        base.Trigger();
        StageMgr.instance.StageEnter(m_TaskData.MapID);
        Complete();
    }
}