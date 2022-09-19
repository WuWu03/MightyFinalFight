using GameFrameWork.Camera;
using UnityEngine;

public class TaskTriggerStageClear: BaseTaskTrigger
{
    public TaskTriggerStageClear(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Trigger()
    {
        Rect vision = CameraMgr.instance.GetVision();
        Vector2 pos = new Vector2(vision.xMax - 0.4f, vision.yMax - 0.5f);
        EffectMgr.instance.PlayDBEffect("Go", pos, 3, 0.5f);
        Complete();
    }
}
