using UnityEngine;

public class TaskTriggerAutoMoveToPos : BaseTaskTrigger
{
    public TaskTriggerAutoMoveToPos(TaskConfigData data) : base(data) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        float x = (float)m_TaskData.Position.Pos.x / 100f;
        float y = (float)m_TaskData.Position.Pos.y / 100f;
        PlayerMgr.instance.player.AutoMoveToPos(new Vector2(x, y));
    }

    public override void Trigger()
    {
        base.Trigger();

        if (!PlayerMgr.instance.player.isAutoMove)
        {
            Complete();
        }
    }
}