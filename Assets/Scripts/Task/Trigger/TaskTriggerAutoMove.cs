using UnityEngine;

public class TaskTriggerAutoMove : BaseTaskTrigger
{
    public TaskTriggerAutoMove(TaskConfigData data) : base(data) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        float x = taskConfigData.Position.Pos.x / 100f;
        float y = taskConfigData.Position.Pos.y / 100f;
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