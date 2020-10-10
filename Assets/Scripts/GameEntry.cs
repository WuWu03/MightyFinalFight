using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using FrameWork.Camera;
using FrameWork.UI;
using System;

public class GameEntry : FrameWorkEntry
{
    protected override void OnInit()
    {
        EffectMgr.Init();
        TaskMgr.Init();
        StageMgr.Init();
        SceneEntityMgr.Init();
        StaticConfig.InitConfig();
    }

    protected override void OnStartGame()
    {
        UIMgr.Ins.Open<RoleSelectPanel>();
    }

    protected override void OnExit()
    {
        EffectMgr.Ins.ShutDown();
        TaskMgr.Ins.ShutDown();
        StageMgr.Ins.ShutDown();
        SceneEntityMgr.Ins.ShutDown();
    }
}