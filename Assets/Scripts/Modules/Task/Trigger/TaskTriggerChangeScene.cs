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
        m_IsLoadScene = false;
    }

    public override void Trigger()
    {
        base.Trigger();

        if (!m_IsLoadScene)
        {
            PlayerMgr.Ins.CanContrl = false;
            UIMgr.Ins.Open<LoadPanel>();
            UIMgr.Ins.GetPanel<LoadPanel>().DOFade(1, 0.3f, 0.5f, ChangeScene);
            m_IsLoadScene = true;
        }
    }

    private void ChangeScene()
    {
        StageMgr.Ins.Enter(m_TaskData.MapID, LoadSceneSuccess);
    }

    private void LoadSceneSuccess()
    {
        UIMgr.Ins.GetPanel<LoadPanel>().DOFade(0f, 0.3f, 0.5f, OnComplete);
    }

    private void OnComplete()
    {
        m_IsComplete = true;
        UIMgr.Ins.Close<LoadPanel>();
    }

    private bool m_IsLoadScene = false;
}