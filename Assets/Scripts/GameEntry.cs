using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Localization;
using GameFrameWork.UI;
using GameFrameWork.Utils;
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
        LoadPanelMgr.Init(manager);
        StaticConfig.InitConfig();
        ConfigDataSheet.Init();

    }

    protected override void OnStartGame()
    {
        LocalizationMgr.instance.SetDefaultLanguage(LanguageType.English);
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.SimplifiedChinese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "SimplifiedChineseLanguageData.bytes")));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.English, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "EnglishLanguageData.bytes")));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.Japanese, new LanguageLoader(PathUtil.FormatPath(config.configDataPath, "JapaneseLanguageData.bytes")));
        LocalizationMgr.instance.ChangeLanguage(LanguageType.SimplifiedChinese);

        CameraMgr.instance.AddOrthographicCamera(CameraName.MainCamera, CameraDepth.MainCamera, CameraTag.MainCamera, 1.0f, LayerName.Map);
        CameraMgr.instance.AddOrthographicCamera(CameraName.RoleCamera, CameraDepth.RoleCamera, CameraTag.Untagged, 1.0f, LayerName.Unit, LayerName.Bullet);
        CameraMgr.instance.AllowAxisFollow(true, false);
        CameraMgr.instance.SetFollowMode(FollowMode.Just);

        UIMgr.instance.Open(UINames.TitlePanel);
    }

    protected override void OnExit()
    {
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
        PlayerMgr.instance.ShutDown();
        HudMgr.instance.ShutDown();
        LoadPanelMgr.instance.ShutDown();
        StaticConfig.ShutDown();
        ConfigDataSheet.ShutDown();
    }
}