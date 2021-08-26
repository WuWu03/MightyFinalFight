using GameFrameWork.Camera;
using UnityEngine;

public class TaskTriggerFinger : BaseTaskTrigger
{
    public TaskTriggerFinger(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        Rect vision = CameraMgr.Ins.GetVision();
        Vector2 pos = new Vector2(vision.xMax - 0.4f, vision.yMax - 0.5f);
        EffectMgr.Ins.PlayEffect("Go", pos, 3, 0.5f);
        Complete();
    }
}