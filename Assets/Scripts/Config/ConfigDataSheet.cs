//===================================================
//作者：GQY                                          
//创建时间：2024-06-06 11:09:24
//备注：此代码为工具生成 请勿手工修改
//===================================================

using GameFrameWork.ConfigData;

/// <summary>
///数据总表
/// </summary>
public static class ConfigDataSheet
{
    public static LevelConfigData[] levelConfigDatas;
	public static RoleConfigData[] roleConfigDatas;
	public static RoleSelectConfigData[] roleSelectConfigDatas;
	public static SceneItemConfigData[] sceneItemConfigDatas;
	public static TalkConfigData[] talkConfigDatas;

	public static void Init()
    {
        ConfigDataHelper.SetResourcesMgr(GameEntry.resourceMgr);
		levelConfigDatas = ConfigDataHelper.LoadConfigData<LevelConfigData>("LevelConfigData.bytes");
		roleConfigDatas = ConfigDataHelper.LoadConfigData<RoleConfigData>("RoleConfigData.bytes");
		roleSelectConfigDatas = ConfigDataHelper.LoadConfigData<RoleSelectConfigData>("RoleSelectConfigData.bytes");
		sceneItemConfigDatas = ConfigDataHelper.LoadConfigData<SceneItemConfigData>("SceneItemConfigData.bytes");
		talkConfigDatas = ConfigDataHelper.LoadConfigData<TalkConfigData>("TalkConfigData.bytes");
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