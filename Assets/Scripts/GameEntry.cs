using GameFrameWork;
using GameFrameWork.Assets;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using GameFrameWork.Localization;
using GameFrameWork.Pool;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using GameFrameWork.Version;
using System;
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
        HudMgr.Init(manager);
        LoadMgr.Init(manager);
        StoryMgr.Init(manager);
    }

    protected override void OnStartGame()
    {
        LocalizationMgr.instance.SetDefaultLanguage(LanguageType.English);
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.SimplifiedChinese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "SimplifiedChineseLanguageData.bytes")));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.English, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "EnglishLanguageData.bytes")));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.Japanese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "JapaneseLanguageData.bytes")));
        LocalizationMgr.instance.ChangeLanguage(LanguageType.SimplifiedChinese);

        StoryMgr.instance.AddStoryBuilder<Story1001>(1001);
        StoryMgr.instance.AddStoryBuilder<Story1002>(1002);
        StoryMgr.instance.AddStoryBuilder<Story1003>(1003);

        if (config.isCheckVersion)
        {
            VersionMgr.instance.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
            UIMgr.instance.Open<VersionView>();
        }
        else
        {
            StartGame();
        }
    }

    private void OnVersionProcessStateChanged(VersionProcessState state, string info, ulong downloadSize, ulong downloadFullSize)
    {
        if (state == VersionProcessState.Success || state == VersionProcessState.DontCheckVersion)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        VersionMgr.instance.onVersionProcessStateChangedEvent -= OnVersionProcessStateChanged;
        LocalizationMgr.instance.ReloadLanguage();
        CameraMgr.instance.AddOrthographicCamera(CameraName.MainCamera, CameraDepth.MainCamera, CameraTag.MainCamera, 1.0f, LayerName.Map);
        CameraMgr.instance.AddOrthographicCamera(CameraName.RoleCamera, CameraDepth.RoleCamera, CameraTag.Untagged, 1.0f, LayerName.Unit, LayerName.Bullet);
        CameraMgr.instance.AllowAxisFollow(true, false);
        CameraMgr.instance.SetFollowMode(FollowMode.Just);
        UIMgr.instance.Close<VersionView>();
        GameObjectPoolMgr.instance.CheckRelease();
        AssetsPool.instance.CheckRelease();
        ReferencePool.ReleaseAll();
        GC.Collect();
        AssetsMgr.instance.InitAssetsMap();
        BehaviourTreeMgr.instance.InitBehaviourTreeData();
        StaticConfig.InitConfig();
        ConfigDataSheet.Init();
        UIMgr.instance.Open<TitleView>();
    }

    protected override void OnExit()
    {
        StoryMgr.instance.ShutDown();
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
        PlayerMgr.instance.ShutDown();
        HudMgr.instance.ShutDown();
        LoadMgr.instance.ShutDown();
        StaticConfig.ShutDown();
        ConfigDataSheet.ShutDown();
    }
}