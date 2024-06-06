
//===================================================
//作者：GQY                                          
//创建时间：2024-06-06 11:09:24
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
	public static RoleConfigData[] roleConfigDatas = null;
	public static RoleSelectConfigData[] roleSelectConfigDatas = null;
	public static SceneItemConfigData[] sceneItemConfigDatas = null;
	public static TalkConfigData[] talkConfigDatas = null;

	public static void Init(string filePath)
	{
		levelConfigDatas = LoadConfigData<LevelConfigData>(filePath, "LevelConfigData.bytes");
		roleConfigDatas = LoadConfigData<RoleConfigData>(filePath, "RoleConfigData.bytes");
		roleSelectConfigDatas = LoadConfigData<RoleSelectConfigData>(filePath, "RoleSelectConfigData.bytes");
		sceneItemConfigDatas = LoadConfigData<SceneItemConfigData>(filePath, "SceneItemConfigData.bytes");
		talkConfigDatas = LoadConfigData<TalkConfigData>(filePath, "TalkConfigData.bytes");
	}

	public static void ShutDown()
	{
		levelConfigDatas = null;
		roleConfigDatas = null;
		roleSelectConfigDatas = null;
		sceneItemConfigDatas = null;
		talkConfigDatas = null;
	}
}
