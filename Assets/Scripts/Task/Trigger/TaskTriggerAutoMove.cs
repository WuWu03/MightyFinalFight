using UnityEngine;

public class TaskTriggerAutoMove : BaseTaskTrigger
{
    public TaskTriggerAutoMove(TaskConfigData data) : base(data) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        float x = (float)mTaskData.Position.Pos.x / 100f;
        float y = (float)mTaskData.Position.Pos.y / 100f;
        PlayerMgr.instance.player.AutoMove(new Vector2(x, y));
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