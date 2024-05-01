using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;


    public static void InitConfig()
    {
        SkillConfig = ResourcesMgr.instance.LoadAssetSync<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResourcesMgr.instance.LoadAssetSync<StageConfig>("ConfigData/StageConfigData");
        TaskConfig = ResourcesMgr.instance.LoadAssetSync<TaskConfig>("ConfigData/TaskData");
    }

    public static void Clear()
    {
    }
}
