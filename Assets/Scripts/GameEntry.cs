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
        ConfigDataHelper.Init(PathUtil.configDataPath);
    }

    protected override void OnStartGame()
    {
        CameraMgr.instance.SetOrthographicSize(1.0f);
        CameraMgr.instance.AllowAxisFollow(true, false);
        CameraMgr.instance.SetFollowMode(FollowMode.Just);
        UIMgr.instance.Open<TitlePanel>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

        }

        if (Input.GetKeyDown(KeyCode.W))
        {

        }

        if (Input.GetKeyDown(KeyCode.E))
        {

        }


        if (Input.GetKeyDown(KeyCode.R))
        {

        }

        if (Input.GetKeyDown(KeyCode.T))
        {

        }

        if (Input.GetKeyDown(KeyCode.Y))
        {

        }

    }

    protected override void OnExit()
    {
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
    }
}