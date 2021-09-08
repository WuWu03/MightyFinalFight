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
        UIMgr.Ins.Open<LoadPanel>().DOFade(0, 1, 0.3f, 0.5f, ChangeScene);
    }

    public override void Trigger()
    {
        base.Trigger();
    }

    private void ChangeScene()
    {
        StageMgr.Ins.Enter(m_TaskData.MapID, LoadSceneSuccess);
    }

    private void LoadSceneSuccess()
    {
        UIMgr.Ins.GetPanel<LoadPanel>().DOFade(1f, 0f, 0.3f, 0.5f, OnComplete);
    }

    private void OnComplete()
    {
        UIMgr.Ins.Close<LoadPanel>();
        Complete();
    }
}