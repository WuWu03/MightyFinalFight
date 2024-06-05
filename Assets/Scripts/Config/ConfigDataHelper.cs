
//===================================================
//作者：GQY                                          
//创建时间：2024-06-05 11:58:34
//备注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using GameFrameWork.ConfigData;

/// <summary>
///数据总表
/// </summary>
public static partial class ConfigDataHelper
{
	public static LevelConfigData[] levelConfigDatas = null;
	public static LocalizationConfigData[] localizationConfigDatas = null;
	public static RoleConfigData[] roleConfigDatas = null;
	public static RoleSelectConfigData[] roleSelectConfigDatas = null;
	public static SceneItemConfigData[] sceneItemConfigDatas = null;
	public static TalkConfigData[] talkConfigDatas = null;

	public static void Init(string filePath)
	{
		levelConfigDatas = LoadConfigData<LevelConfigData>(filePath, "LevelConfigData");
		localizationConfigDatas = LoadConfigData<LocalizationConfigData>(filePath, "LocalizationConfigData");
		roleConfigDatas = LoadConfigData<RoleConfigData>(filePath, "RoleConfigData");
		roleSelectConfigDatas = LoadConfigData<RoleSelectConfigData>(filePath, "RoleSelectConfigData");
		sceneItemConfigDatas = LoadConfigData<SceneItemConfigData>(filePath, "SceneItemConfigData");
		talkConfigDatas = LoadConfigData<TalkConfigData>(filePath, "TalkConfigData");
	}
}
