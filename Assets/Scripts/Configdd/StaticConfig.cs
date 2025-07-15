using GameFrameWork.Assets;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;

    public static void InitConfig()
    {
        SkillConfig = AssetsMgr.instance.LoadAssetSync<SkillConfig>("ConfigData/SkillConfig.asset");
        StageConfig = AssetsMgr.instance.LoadAssetSync<StageConfig>("ConfigData/StageConfig.asset");
        TaskConfig = AssetsMgr.instance.LoadAssetSync<TaskConfig>("ConfigData/TaskConfig.asset");
    }

    public static void ShutDown()
    {
        SkillConfig = null;
        StageConfig = null;
        TaskConfig = null;
    }
}
