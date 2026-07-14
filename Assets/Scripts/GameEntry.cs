using System;
using UnityEngine;
using WuWuFramework;
using WuWuFramework.BehaviourTree;
using WuWuFramework.Camera;
using WuWuFramework.ConfigData;
using WuWuFramework.Download;
using WuWuFramework.Event;
using WuWuFramework.Fsm;
using WuWuFramework.GameEntity;
using WuWuFramework.Input;
using WuWuFramework.Localization;
using WuWuFramework.Pool;
using WuWuFramework.Resources;
using WuWuFramework.Scene;
using WuWuFramework.Sound;
using WuWuFramework.Timer;
using WuWuFramework.UI;
using WuWuFramework.Utils;
using WuWuFramework.Version;
using WuWuFramework.WebRequest;

public class GameEntry : WuWuFrameworkEntry
{
    private static ILocalizationMgr s_LocalizationMgr;
    private static IResourcesMgr s_ResourcesMgr;
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
    private static IGameEntityMgr s_EntityMgr;
    private static IInputMgr s_InputMgr;
    private static ISceneMgr s_SceneMgr;
    private static ITimerMgr s_TimerMgr;
    private static IConfigDataMgr s_ConfigDataMgr;
    private static ICameraMgr s_CameraMgr;

    public static ILocalizationMgr localizationMgr
    {
        get
        {
            if (s_LocalizationMgr == null)
            {
                s_LocalizationMgr = WuWuFrameworkMgr.GetModule<ILocalizationMgr>();
                s_LocalizationMgr.SetResourceManager(resourceMgr);
            }

            return s_LocalizationMgr;
        }
    }

    public static IResourcesMgr resourceMgr
    {
        get
        {
            s_ResourcesMgr ??= WuWuFrameworkMgr.GetModule<IResourcesMgr>();
            return s_ResourcesMgr;
        }
    }

    public static IVersionMgr versionMgr
    {
        get
        {
            if (s_VersionMgr == null)
            {
                s_VersionMgr = WuWuFrameworkMgr.GetModule<IVersionMgr>();
                s_VersionMgr.SetDownloadMgr(downloadMgr);
            }

            return s_VersionMgr;
        }
    }

    public static IDownloadMgr downloadMgr
    {
        get
        {
            s_DownloadMgr ??= WuWuFrameworkMgr.GetModule<IDownloadMgr>();
            return s_DownloadMgr;
        }
    }

    public static IWebRequestMgr webRequestMgr
    {
        get
        {
            s_WebRequestMgr ??= WuWuFrameworkMgr.GetModule<IWebRequestMgr>();
            return s_WebRequestMgr;
        }
    }

    public static IUIMgr uiMgr
    {
        get
        {
            if (s_UIMgr == null)
            {
                s_UIMgr = WuWuFrameworkMgr.GetModule<IUIMgr>();
                s_UIMgr.SetGameObjectPoolMgr(gameObjectPoolMgr);
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
                s_GameObjectPoolMgr = WuWuFrameworkMgr.GetModule<IGameObjectPoolMgr>();
                s_GameObjectPoolMgr.SetResourcePoolMgr(resourcePoolMgr);
            }

            return s_GameObjectPoolMgr;
        }
    }

    public static IEventMgr eventMgr
    {
        get
        {
            s_EventMgr ??= WuWuFrameworkMgr.GetModule<IEventMgr>();
            return s_EventMgr;
        }
    }
    public static IResourcePoolMgr resourcePoolMgr
    {
        get
        {
            if (s_ResourcePoolMgr == null)
            {
                s_ResourcePoolMgr = WuWuFrameworkMgr.GetModule<IResourcePoolMgr>();
                s_ResourcePoolMgr.SetResourcesMgr(resourceMgr);
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
                s_BehaviourTreeMgr = WuWuFrameworkMgr.GetModule<IBehaviourTreeMgr>();
                s_BehaviourTreeMgr.SetResourcesMgr(resourceMgr);
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
                s_SoundMgr = WuWuFrameworkMgr.GetModule<ISoundMgr>();
                s_SoundMgr.SetResourcePoolMgr(resourcePoolMgr);
            }

            return s_SoundMgr;
        }
    }

    public static IFsmMgr fsmMgr
    {
        get
        {
            s_FsmMgr ??= WuWuFrameworkMgr.GetModule<IFsmMgr>();
            return s_FsmMgr;
        }
    }

    public static IGameEntityMgr entityMgr
    {
        get
        {
            if (s_EntityMgr == null)
            {
                s_EntityMgr = WuWuFrameworkMgr.GetModule<IGameEntityMgr>();
                s_EntityMgr.SetGameObjectPoolMgr(gameObjectPoolMgr);
            }

            return s_EntityMgr;
        }
    }

    public static IInputMgr inputMgr
    {
        get
        {
            if (s_InputMgr == null)
            {
                s_InputMgr = WuWuFrameworkMgr.GetModule<IInputMgr>();
                s_InputMgr.SetResourcesMgr(s_ResourcesMgr);
            }
            
            return s_InputMgr;
        }
    }

    public static ISceneMgr sceneMgr
    {
        get
        {
            if (s_SceneMgr == null)
            {
                s_SceneMgr = WuWuFrameworkMgr.GetModule<ISceneMgr>();
                s_SceneMgr.SetResourcesMgr(resourceMgr);
            }

            return s_SceneMgr;
        }
    }

    public static ITimerMgr timerMgr
    {
        get
        {
            s_TimerMgr ??= WuWuFrameworkMgr.GetModule<ITimerMgr>();
            return s_TimerMgr;
        }
    }

    public static IConfigDataMgr configDataMgr
    {
        get
        {
            s_ConfigDataMgr ??= WuWuFrameworkMgr.GetModule<IConfigDataMgr>();
            s_ConfigDataMgr.SetResourcesMgr(resourceMgr);
            return s_ConfigDataMgr;
        }
    }

    public static ICameraMgr cameraMgr
    {
        get
        {
            s_CameraMgr ??= WuWuFrameworkMgr.GetModule<ICameraMgr>();
            return s_CameraMgr;
        }
    }

    protected override void OnInit(GameObject manager)
    {

    }

    protected override void OnStartGame()
    {
        configDataMgr.Cache<LevelConfigData>();
        configDataMgr.Cache<RoleConfigData>();
        configDataMgr.Cache<RoleSelectConfigData>();
        configDataMgr.Cache<SceneItemConfigData>();
        configDataMgr.Cache<StoryConfigData>();
        configDataMgr.Cache<TalkConfigData>();

        LanguageText.SetLocalizationMgr(localizationMgr);
        localizationMgr.SetDefaultLanguage(LanguageType.English);
        localizationMgr.AddLanguageLoader(LanguageType.SimplifiedChinese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, AssetPathDefine.SimplifiedChinese)));
        localizationMgr.AddLanguageLoader(LanguageType.English, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, AssetPathDefine.English)));
        localizationMgr.AddLanguageLoader(LanguageType.Japanese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, AssetPathDefine.Japanese)));
        localizationMgr.ChangeLanguage(LanguageType.SimplifiedChinese);

        StoryMgr.instance.AddStoryBuilder<Story1001>(1001);
        StoryMgr.instance.AddStoryBuilder<Story1002>(1002);
        StoryMgr.instance.AddStoryBuilder<Story1003>(1003);

        if (config.isCheckVersion)
        {
            versionMgr.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
            uiMgr.Open<VersionView>();
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
        inputMgr.AddInputController(InputScheme.Keyboard);
        inputMgr.AddInputController(InputScheme.Xbox);
        inputMgr.SetCurrScheme(InputScheme.Keyboard);
        versionMgr.onVersionProcessStateChangedEvent -= OnVersionProcessStateChanged;
        localizationMgr.ReloadLanguage();
        CameraFollowMgr.instance.Init();
        uiMgr.Close<VersionView>();
        gameObjectPoolMgr.CheckRelease();
        resourcePoolMgr.CheckRelease();
        ReferencePool.ReleaseAll();
        GC.Collect();
        resourceMgr.InitAssetsMap();
        StaticConfig.InitConfig();
        uiMgr.Open<TitleView>();
    }

    protected override void OnExit()
    {
        StaticConfig.ShutDown();
    }
}