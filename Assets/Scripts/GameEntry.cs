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
        StaticConfig.InitConfig();
    }

    protected override void OnStartGame()
    {
        UIMgr.Ins.Open<RoleSelectPanel>();
    }

    protected override void OnExit()
    {

    }
}