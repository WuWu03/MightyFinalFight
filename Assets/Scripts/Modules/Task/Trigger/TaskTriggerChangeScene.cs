using GameFrameWork.Scene;
using GameFrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerChangeScene : BaseTaskTrigger
{
    public TaskTriggerChangeScene(TaskConfigData data) : base(data) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Trigger()
    {
        base.Trigger();
        StageMgr.Ins.StageEnter(m_TaskData.MapID);
        Complete();
    }
}