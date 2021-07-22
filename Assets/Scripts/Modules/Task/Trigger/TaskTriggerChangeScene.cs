using GameFrameWork.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerChangeScene : BaseTaskTrigger
{
    public TaskTriggerChangeScene(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        StageMgr.Ins.Enter(m_TaskData.MapID, LoadSceneSuccess);
    }

    private void LoadSceneSuccess()
    {
        m_IsComplete = true;
    }
}