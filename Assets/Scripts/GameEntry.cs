using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Localization;
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
        HudMgr.Init(manager);
        StaticConfig.InitConfig();
        ConfigDataHelper.Init(PathUtil.configDataPath);
    }

    protected override void OnStartGame()
    {
        LocalizationMgr.instance.SetDefaultLanguage(LanguageType.English);
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.SimplifiedChinese, new SimplifiedChineseLanguageLoader(AssetPathDefine.SimplifiedChinesePath));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.English, new EnglishLanguageLoader(AssetPathDefine.EnglishPath));
        LocalizationMgr.instance.AddLanguageLoader(LanguageType.Japanese, new JapaneseLanguageLoader(AssetPathDefine.JapanesePath));
        LocalizationMgr.instance.ChangeLanguage(LanguageType.SimplifiedChinese);

        CameraMgr.instance.AddOrthographicCamera(CameraName.MainCamera, CameraDepth.MainCamera, CameraTag.MainCamera, 1.0f, LayerName.Map);
        CameraMgr.instance.AddOrthographicCamera(CameraName.RoleCamera, CameraDepth.RoleCamera, CameraTag.Untagged, 1.0f, LayerName.Unit, LayerName.Bullet);
        CameraMgr.instance.AllowAxisFollow(true, false);
        CameraMgr.instance.SetFollowMode(FollowMode.Just);

        UIMgr.instance.Open<TitlePanel>();
    }

    protected override void OnExit()
    {
        ConfigDataHelper.ShutDown();
        EffectMgr.instance.ShutDown();
        TaskMgr.instance.ShutDown();
        StageMgr.instance.ShutDown();
        SceneEntityMgr.instance.ShutDown();
        HudMgr.instance.ShutDown();
    }
}