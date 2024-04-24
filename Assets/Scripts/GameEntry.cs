using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Resources;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using System;
using UnityEngine;
using UnityEngine.U2D;

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

        SpriteAtlasManager.atlasRequested += AtlasRequested;
    }

    protected override void OnStartGame()
    {
        CameraMgr.instance.SetOrthographicSize(1.0f);
        CameraMgr.instance.AllowAxisFollow(true, false);
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

    private void AtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        string atlasPath = PathUtil.FormatPath(PathUtil.GetUIAtlasPath(), tag);
        ResourcesMgr.instance.LoadAssetAsync(atlasPath, (string assetPath, UnityEngine.Object obj, object[] args) =>
        {
            action(obj as SpriteAtlas);
        });
    }
}