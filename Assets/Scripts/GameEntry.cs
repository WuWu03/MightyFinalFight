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
        StaticConfig.InitConfig();
        TaskMgr.Init();
    }

    protected override void OnStartGame()
    {
        UIMgr.Ins.Open<RoleSelectPanel>();
    }

    protected override void OnExit()
    {

    }
}