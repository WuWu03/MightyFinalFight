using GameFrameWork.Assets;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;

    public static void InitConfig()
    {
        SkillConfig = AssetsMgr.instance.LoadAssetSync<SkillConfig>("ConfigData/SkillData.asset");
        StageConfig = AssetsMgr.instance.LoadAssetSync<StageConfig>("ConfigData/StageConfigData.asset");
        TaskConfig = AssetsMgr.instance.LoadAssetSync<TaskConfig>("ConfigData/TaskData.asset");
    }

    public static void Clear()
    {

    }
}
