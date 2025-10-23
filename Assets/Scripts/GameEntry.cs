using System;
using GameFrameWork;
using GameFrameWork.Assets;
using GameFrameWork.Audio;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Download;
using GameFrameWork.Event;
using GameFrameWork.Fsm;
using GameFrameWork.GameEntity;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.Pool;
using GameFrameWork.Scene;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using GameFrameWork.Version;
using GameFrameWork.WebRequest;
using UnityEngine;

public class GameEntry : GameFrameWorkEntry
{
    private static Transform s_GameEntry;
    private static ILocalizationMgr s_LocalizationMgr;
    private static IResourceMgr s_ResourceMgr;
    private static IVersionMgr s_VersionMgr;
    private static IDownloadMgr s_DownloadMgr;
    private static IWebRequestMgr s_WebRequestMgr;
    private static IUIMgr s_UIMgr;
    private static IGameObjectPoolMgr s_GameObjectPoolMgr;
    private static IEventMgr s_EventMgr;
    private static IResourcePoolMgr s_ResourcePoolMgr;
    private static IBehaviourTreeMgr s_BehaviourTreeMgr;
    private static ISoundMgr s_SoundMgr;
    private static IFsmMgr s_FsmMgr;
    private static IEntityMgr s_EntityMgr;
    private static IInputMgr s_InputMgr;
    private static ISceneMgr s_SceneMgr;
    private static ITimerMgr s_TimerMgr;
    
    public static ILocalizationMgr localizationMgr
    {
        get
        {
            if (s_LocalizationMgr == null)
            {
                s_LocalizationMgr = GameFrameWorkMgr.GetModule<ILocalizationMgr>();
                s_LocalizationMgr.SetResourceManager(resourceMgr);
            }

            return s_LocalizationMgr;
        }
    }
    
    public static IResourceMgr resourceMgr
    {
        get
        {
            s_ResourceMgr ??= GameFrameWorkMgr.GetModule<IResourceMgr>();
            return s_ResourceMgr;
        }
    }

    public static IVersionMgr versionMgr
    {
        get
        {
            if (s_VersionMgr == null)
            {
                s_VersionMgr = GameFrameWorkMgr.GetModule<IVersionMgr>();
                s_VersionMgr.SetMgr(downloadMgr, webRequestMgr);
            }
            
            return s_VersionMgr;
        }
    }
    
    public static IDownloadMgr downloadMgr
    {
        get
        {
            s_DownloadMgr ??= GameFrameWorkMgr.GetModule<IDownloadMgr>();
            return s_DownloadMgr;
        }
    }
    
    public static IWebRequestMgr webRequestMgr
    {
        get
        {
            s_WebRequestMgr ??= GameFrameWorkMgr.GetModule<IWebRequestMgr>();
            return s_WebRequestMgr;
        }
    }

    public static IUIMgr uiMgr
    {
        get
        {
            if (s_UIMgr == null)
            {
                s_UIMgr = GameFrameWorkMgr.GetModule<IUIMgr>();
                s_UIMgr.SetMgr(gameObjectPoolMgr, eventMgr);
            }

            return s_UIMgr;
        }
    }

    public static IGameObjectPoolMgr gameObjectPoolMgr
    {
        get
        {
            if (s_GameObjectPoolMgr == null)
            {
                s_GameObjectPoolMgr = GameFrameWorkMgr.GetModule<IGameObjectPoolMgr>();
                s_GameObjectPoolMgr.SetResourcePoolMgr(resourcePoolMgr, s_GameEntry);
            }

            return s_GameObjectPoolMgr;
        }
    }

    public static IEventMgr eventMgr
    {
        get
        {
            s_EventMgr ??= GameFrameWorkMgr.GetModule<IEventMgr>();
            return s_EventMgr;
        }
    }
    public static IResourcePoolMgr resourcePoolMgr
    {
        get
        {
            if (s_ResourcePoolMgr == null)
            {
                s_ResourcePoolMgr = GameFrameWorkMgr.GetModule<IResourcePoolMgr>();
                s_ResourcePoolMgr.SetResourceMgr(resourceMgr, s_GameEntry);
            }

            return s_ResourcePoolMgr;
        }
    }

    public static IBehaviourTreeMgr behaviourTreeMgr
    {
        get
        {
            if (s_BehaviourTreeMgr == null)
            {
                s_BehaviourTreeMgr = GameFrameWorkMgr.GetModule<IBehaviourTreeMgr>();
                s_BehaviourTreeMgr.SetResourceMgr(resourceMgr);
            }

            return s_BehaviourTreeMgr;
        }
    }

    public static ISoundMgr soundMgr
    {
        get
        {
            if (s_SoundMgr == null)
            {
                s_SoundMgr = GameFrameWorkMgr.GetModule<ISoundMgr>();
                s_SoundMgr.SetResourcePoolMgr(resourcePoolMgr, s_GameEntry);
            }
            
            return s_SoundMgr;
        }
    }
    
    public static IFsmMgr fsmMgr
    {
        get
        {
            s_FsmMgr ??= GameFrameWorkMgr.GetModule<IFsmMgr>();
            return s_FsmMgr;
        }
    }

    public static IEntityMgr entityMgr
    {
        get
        {
            if (s_EntityMgr == null)
            {
                s_EntityMgr = GameFrameWorkMgr.GetModule<IEntityMgr>();
                s_EntityMgr.SetGameObjectPoolMgr(gameObjectPoolMgr, s_GameEntry);
            }

            return s_EntityMgr;
        }
    }

    public static IInputMgr inputMgr
    {
        get
        {
            s_InputMgr ??= GameFrameWorkMgr.GetModule<IInputMgr>();
            return s_InputMgr;
        }
    }

    public static ISceneMgr sceneMgr
    {
        get
        {
            if (s_SceneMgr == null)
            {
                s_SceneMgr = GameFrameWorkMgr.GetModule<ISceneMgr>();
                s_SceneMgr.SetResourceMgr(resourceMgr);
            }

            return s_SceneMgr;
        }
    }

    public static ITimerMgr timerMgr
    {
        get
        {
            s_TimerMgr ??= GameFrameWorkMgr.GetModule<ITimerMgr>();
            return s_TimerMgr;
        }
    }
    
    protected override void OnInit(GameObject manager)
    {
        s_GameEntry = gameObject.transform;
        EffectMgr.Init(manager);
        TaskMgr.Init(manager);
        StageMgr.Init(manager);
        SceneEntityMgr.Init(manager);
        PlayerMgr.Init(manager);
        HudMgr.Init(manager);
        LoadMgr.Init(manager);
        StoryMgr.Init(manager);
        CameraMgr.Init(manager);
    }

    protected override void OnStartGame()
    {
        LanguageText.SetLocalizationMgr(localizationMgr);
        localizationMgr.SetDefaultLanguage(LanguageType.English);
        localizationMgr.AddLanguageLoader(LanguageType.SimplifiedChinese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "SimplifiedChineseLanguageData.bytes")));
        localizationMgr.AddLanguageLoader(LanguageType.English, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "EnglishLanguageData.bytes")));
        localizationMgr.AddLanguageLoader(LanguageType.Japanese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "JapaneseLanguageData.bytes")));
        localizationMgr.ChangeLanguage(LanguageType.SimplifiedChinese);

        StoryMgr.instance.AddStoryBuilder<Story1001>(1001);
        StoryMgr.instance.AddStoryBuilder<Story1002>(1002);
        StoryMgr.instance.AddStoryBuilder<Story1003>(1003);

        if (config.isCheckVersion)
        {
            versionMgr.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
            GameFrameWorkMgr.GetModule<IUIMgr>().Open<VersionView>();
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
        versionMgr.onVersionProcessStateChangedEvent -= OnVersionProcessStateChanged;
        localizationMgr.ReloadLanguage();
        CameraMgr.instance.AddOrthographicCamera(CameraName.MainCamera, CameraDepth.MainCamera, CameraTag.MainCamera, 1.0f, LayerName.Map);
        CameraMgr.instance.AddOrthographicCamera(CameraName.RoleCamera, CameraDepth.RoleCamera, CameraTag.Untagged, 1.0f, LayerName.Unit, LayerName.Bullet);
        CameraMgr.instance.AllowAxisFollow(true, false);
        CameraMgr.instance.SetFollowMode(CameraFollow.FollowMode.Just);
        uiMgr.Close<VersionView>();
        gameObjectPoolMgr.CheckRelease();
        resourcePoolMgr.CheckRelease();
        ReferencePool.ReleaseAll();
        GC.Collect();
        resourceMgr.InitAssetsMap();
        StaticConfig.InitConfig();
        ConfigDataSheet.Init();
        uiMgr.Open<TitleView>();
    }

    protected override void OnExit()
    {
        CameraMgr.instance.ShutDown();
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