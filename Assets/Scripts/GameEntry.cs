using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.UI;
using System;

public class GameEntry : GameFrameWorkEntry
{
    protected override void OnInit()
    {
        EffectMgr.Init(m_Manager);
        TaskMgr.Init(m_Manager);
        StageMgr.Init(m_Manager);
        SceneEntityMgr.Init(m_Manager);
        StaticConfig.InitConfig();
        CameraMgr.Ins.SetLerp(false);
    }

    protected override void OnStartGame()
    {
        //Mathf.Lerp()
        UIMgr.Ins.Open<RoleSelectPanel>();
        //UIMgr.Ins.Open<TestMailPanel>();
    }

    protected override void OnExit()
    {
        EffectMgr.Ins.ShutDown();
        TaskMgr.Ins.ShutDown();
        StageMgr.Ins.ShutDown();
        SceneEntityMgr.Ins.ShutDown();
    }
}