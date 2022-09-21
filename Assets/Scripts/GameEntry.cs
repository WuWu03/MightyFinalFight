using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.UI;
using System;
using GameFrameWork.Utilities;

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
        DataHelper.Init(PathUtil.configDataPath);
    }

    protected override GameFrameWork.UI.UIResPath InitUIResPath()
    {
        return new UIResPath();
    }

    protected override void OnStartGame()
    {
        CameraMgr.instance.SetFollowMode(FollowMode.Just);
        UIMgr.instance.Open<RoleSelectPanel>();
    }

    protected override void OnExit()
    {
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
    }
}