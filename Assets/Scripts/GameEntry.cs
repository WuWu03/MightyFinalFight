using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;

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

    protected override void OnExit()
    {
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
    }
}