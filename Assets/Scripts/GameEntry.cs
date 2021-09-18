using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.UI;
using System;

public class GameEntry : GameFrameWorkEntry
{
    protected override void OnInit(GameObject manager)
    {
        EffectMgr.Init(manager);
        TaskMgr.Init(manager);
        StageMgr.Init(manager);
        SceneEntityMgr.Init(manager);
        PlayerMgr.Init(manager);
        StaticConfig.InitConfig();
    }

    protected override GameFrameWork.UI.UIResPath InitUIResPath()
    {
        return new UIResPath();
    }

    protected override void OnStartGame()
    {
        CameraMgr.Ins.SetFollowMode(FollowMode.Just);
        UIMgr.Ins.Open<TitlePanel>();
    }

    protected override void OnExit()
    {
        EffectMgr.Ins.ShutDown();
        TaskMgr.Ins.ShutDown();
        StageMgr.Ins.ShutDown();
        SceneEntityMgr.Ins.ShutDown();
    }
}